using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableStickerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public StickerSO myStickerData;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    private WeaponSlotUI currentHoveredSlot;
    private Coroutine movementCoroutine;

    // 记录它的散落归宿，如果没装上，就回这里
    private Vector2 scatterPosition;
    private float scatterRotationZ;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // 由 Panel 调用，初始化数据和落点
    public void Initialize(StickerSO data, Vector2 targetLocalPos, float targetRotZ)
    {
        myStickerData = data;
        scatterPosition = targetLocalPos;
        scatterRotationZ = targetRotZ;
        
        // 可选：在这里播放一个从屏幕外飞入到散落点的初始化动画
        MoveToPosition(scatterPosition, scatterRotationZ, 0.5f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标放上去（未拖拽）：显示全部效果
        StickerTooltipPanel.Instance.ShowTooltip(myStickerData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 如果没有在拖拽，移开鼠标就隐藏
        if (movementCoroutine == null && currentHoveredSlot == null)
            StickerTooltipPanel.Instance.HideTooltip();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        transform.SetAsLastSibling();
        rectTransform.localRotation = Quaternion.identity;
        canvasGroup.blocksRaycasts = false; 

        // 🌟 刚被抓起来的时候，保持显示全部效果
        StickerTooltipPanel.Instance.ShowTooltip(myStickerData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        WeaponSlotUI foundSlot = null;
        foreach (var result in results)
        {
            if (result.gameObject.TryGetComponent(out foundSlot)) break;
        }

        // 状态发生了变化（进入新槽位，或者离开了槽位）
        if (foundSlot != currentHoveredSlot)
        {
            if (currentHoveredSlot != null) 
            {
                currentHoveredSlot.SetHighlight(false);
            }
            
            if (foundSlot != null)
            {
                // 如果类型兼容，亮起光圈，并只显示该槽位的描述！
                if (myStickerData.compatibleSlot == StickerSlotType.Any || myStickerData.compatibleSlot == foundSlot.mySlotType)
                {
                    foundSlot.SetHighlight(true);
                    StickerTooltipPanel.Instance.ShowTooltip(myStickerData, foundSlot.mySlotType);
                }
            }
            else
            {
                // 🌟 从槽位上挪开了（悬空了），恢复显示全部描述！
                StickerTooltipPanel.Instance.ShowTooltip(myStickerData);
            }
            
            currentHoveredSlot = foundSlot;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // 恢复射线阻挡
        StickerTooltipPanel.Instance.HideTooltip(); // 拖拽结束，必定隐藏气泡

        if (currentHoveredSlot != null)
        {
            currentHoveredSlot.SetHighlight(false);
            bool success = InventoryManager.Instance.TryEquipSticker(myStickerData, currentHoveredSlot.mySlotType);
            
            if (success) 
            {
                currentHoveredSlot.UpdateVisual(myStickerData);
                // 装备成功，销毁散落区的自己
                Destroy(gameObject); 
                return;
            }
        }
        
        // 🌟 如果没有对准槽位，或者安装失败：弹回地面，并恢复之前的散落旋转角度！
        MoveToPosition(scatterPosition, scatterRotationZ, 0.3f);
        currentHoveredSlot = null;
    }

    // 自定义的平滑移动与旋转协程（替代 DOTween）
    private void MoveToPosition(Vector2 targetLocalPos, float targetRotZ, float duration)
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        movementCoroutine = StartCoroutine(LerpTransformRoutine(targetLocalPos, targetRotZ, duration));
    }

    private IEnumerator LerpTransformRoutine(Vector2 targetPos, float targetRot, float time)
    {
        Vector2 startPos = rectTransform.localPosition;
        Quaternion startRot = rectTransform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, targetRot);
        
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            // 使用 SmoothStep 增加动画的呼吸感和物理缓冲感
            float smoothT = Mathf.SmoothStep(0, 1, t); 
            
            rectTransform.localPosition = Vector2.Lerp(startPos, targetPos, smoothT);
            rectTransform.localRotation = Quaternion.Lerp(startRot, endRot, smoothT);
            yield return null;
        }
    }
}