using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 🌟 新增：IPointerEnterHandler, IPointerExitHandler 接口用于处理地上贴纸的鼠标悬停
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class DraggableStickerUI
    : MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler
{
    [Header("数据")]
    public float rotationLerpSpeed = 15f;
    private StickerSO myData;
    private Vector2 lastMousePosition;
    private Vector2 throwVelocity;
    private Rigidbody2D rb;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image myImage;

    private bool isDragging = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(StickerSO data)
    {
        myData = data;
        if (myImage != null)
            myImage.sprite = data.icon;
    }

    private void Update()
    {
        if (isDragging)
        {
            float currentZ = rectTransform.eulerAngles.z;
            float targetZ = 0f;
            float newZ = Mathf.LerpAngle(
                currentZ,
                targetZ,
                Time.unscaledDeltaTime * rotationLerpSpeed
            );
            rectTransform.rotation = Quaternion.Euler(0, 0, newZ);
        }
    }

    // 🌟 新增：鼠标放在地上未安装的贴纸上时，显示全属性
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging && StickerTooltipPanel.Instance != null)
        {
            StickerTooltipPanel.Instance.OnShow(myData, null);
        }
    }

    // 🌟 新增：鼠标挪开时隐藏
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging && StickerTooltipPanel.Instance != null)
        {
            StickerTooltipPanel.Instance.OnHide();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();

        lastMousePosition = eventData.position;

        // 拖起来的瞬间，确保显示全属性 Tooltip
        if (StickerTooltipPanel.Instance != null)
        {
            StickerTooltipPanel.Instance.OnShow(myData, null);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. 移动贴纸坐标
        if (
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint
            )
        )
        {
            rectTransform.position = worldPoint;
        }

        if (Time.unscaledDeltaTime > 0)
        {
            throwVelocity = (eventData.position - lastMousePosition) / Time.unscaledDeltaTime;
        }
        lastMousePosition = eventData.position;

        // 🌟 2. 核心动态切换逻辑：拖拽中实时发射射线探测
        if (StickerTooltipPanel.Instance != null)
        {
            WeaponSlotUI targetSlot = DetectSlotUnderMouse(eventData);
            if (targetSlot != null)
            {
                // 悬停在槽位上了！只显示对应槽位的属性
                StickerTooltipPanel.Instance.OnShow(myData, targetSlot.mySlotType);
            }
            else
            {
                // 挪出槽位了，或者在半空中，恢复显示全部属性
                StickerTooltipPanel.Instance.OnShow(myData, null);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        // 松手时，隐藏 Tooltip
        if (StickerTooltipPanel.Instance != null)
        {
            StickerTooltipPanel.Instance.OnHide();
        }

        WeaponSlotUI targetSlot = DetectSlotUnderMouse(eventData);

        if (targetSlot != null)
        {
            StickerSO existingSticker = targetSlot.GetCurrentSticker();

            if (existingSticker != null)
            {
                StickerInventoryPanel panel = UIManager.Instance.GetPanel<StickerInventoryPanel>();
                if (panel != null && panel.gameObject.activeInHierarchy)
                {
                    InventoryManager.Instance.UnequipSticker(targetSlot.mySlotType);
                    Vector2 slotScreenPos = RectTransformUtility.WorldToScreenPoint(
                        eventData.pressEventCamera,
                        targetSlot.transform.position
                    );
                    panel.SpawnStickerAtMouse(
                        existingSticker,
                        slotScreenPos,
                        eventData.pressEventCamera
                    );
                }
            }

            bool success = InventoryManager.Instance.TryEquipSticker(myData, targetSlot.mySlotType);
            if (success)
            {
                targetSlot.UpdateVisual(myData);

                // 贴纸被成功装入，由于要销毁自身，为了防止 Tooltip 卡在屏幕上，再保险隐藏一次
                if (StickerTooltipPanel.Instance != null)
                    StickerTooltipPanel.Instance.OnHide();

                Destroy(gameObject);
                return;
            }
            else
            {
                Debug.LogWarning($"⚠️ 装配失败：这枚贴纸不能放进 [{targetSlot.mySlotType}] 槽位！");
            }
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = throwVelocity * 0.02f;
        rb.angularVelocity = Random.Range(-100f, 100f);
    }

    private WeaponSlotUI DetectSlotUnderMouse(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            WeaponSlotUI slot = result.gameObject.GetComponentInParent<WeaponSlotUI>();
            if (slot != null)
                return slot;
        }
        return null;
    }
}
