using UnityEngine;
using UnityEngine.EventSystems; // 必须引入事件系统
using UnityEngine.UI;

// 添加三个接口：悬停进入、悬停离开、点击
public class WeaponSlotUI
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    [Header("槽位配置")]
    public StickerSlotType mySlotType;

    [Header("UI 组件绑定")]
    public Image backgroundImage;
    public Image highlightImage;
    public Image equippedIcon;

    // 🌟 新增：记录当前槽位里装的是什么贴纸
    private StickerSO currentSticker;

    private void Awake()
    {
        SetHighlight(false);
    }

    public void SetHighlight(bool isOn)
    {
        if (highlightImage != null)
            highlightImage.enabled = isOn;
    }

    public void UpdateVisual(StickerSO sticker)
    {
        currentSticker = sticker; // 存下来，留给交互用

        if (equippedIcon == null)
            return;

        if (sticker != null)
        {
            equippedIcon.sprite = sticker.icon;
            equippedIcon.enabled = true;
            if (backgroundImage != null)
                backgroundImage.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            equippedIcon.sprite = null;
            equippedIcon.enabled = false;
            if (backgroundImage != null)
                backgroundImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    // 🌟 鼠标放上去时：显示当前槽位的专属 Tooltip！
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSticker != null)
        {
            StickerTooltipPanel.Instance.OnShow(currentSticker, mySlotType);
        }
    }

    // 🌟 鼠标挪开时：隐藏 Tooltip
    public void OnPointerExit(PointerEventData eventData)
    {
        StickerTooltipPanel.Instance.OnHide();
    }

    // 🌟 鼠标点击时：一键卸下贴纸！
    // 必须继承 IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentSticker != null)
        {
            // 1. 先把当前贴纸的数据存下来
            StickerSO stickerToDrop = currentSticker;

            // 2. 告诉大管家：扣掉底层数据
            InventoryManager.Instance.UnequipSticker(mySlotType);

            // 3. 本地槽位立刻清空图片显示
            UpdateVisual(null);

            // 4. 获取面板，并在鼠标当前位置喷出一个贴纸！
            StickerInventoryPanel panel = UIManager.Instance.GetPanel<StickerInventoryPanel>();
            if (panel != null && panel.gameObject.activeInHierarchy)
            {
                // 如果你的 Canvas 是 Screen Space - Overlay，eventData.pressEventCamera 会是 null，这是正常的
                panel.SpawnStickerAtMouse(
                    stickerToDrop,
                    eventData.position,
                    eventData.pressEventCamera
                );
            }
        }
    }

    public StickerSO GetCurrentSticker()
    {
        return currentSticker;
    }
}
