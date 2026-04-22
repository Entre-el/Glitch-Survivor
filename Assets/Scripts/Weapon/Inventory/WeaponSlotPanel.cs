using UnityEngine;
using UnityEngine.UI;

// 挂在武器的四个圆形槽位上
public class WeaponSlotUI : MonoBehaviour
{
    [Header("槽位配置")]
    public StickerSlotType mySlotType; // 这个槽位是 Fire, Pierce, Crit 还是 Fade？

    [Header("UI 组件绑定")]
    public Image backgroundImage;        // 圆形底图（空槽位时显示的背景）
    public Image highlightImage;       // 拖拽到上方时的发光特效图（光圈）
    public Image equippedIcon;         // 装备成功后显示的贴纸图标

    private void Awake()
    {
        // 初始状态：关闭高亮
        SetHighlight(false);
    }

    /// <summary>
    /// 控制高亮光圈的开关（由 DraggableStickerUI 在 OnDrag 时调用）
    /// </summary>
    public void SetHighlight(bool isOn)
    {
        if (highlightImage != null) 
        {
            highlightImage.enabled = isOn;
        }
    }

    /// <summary>
    /// 更新槽位的视觉表现（由 InventoryManager 装备/卸下成功后调用）
    /// </summary>
    public void UpdateVisual(StickerSO sticker)
    {
        if (sticker != null)
        {
            // 🌟 状态 A：有贴纸装入
            equippedIcon.sprite = sticker.icon;
            equippedIcon.enabled = true;

            // 为了突出贴纸，可以把底图设为半透明，或者直接隐藏
            if (backgroundImage != null) 
            {
                backgroundImage.color = new Color(1f, 1f, 1f, 0.3f); 
            }
        }
        else
        {
            // 🌟 状态 B：空槽位（贴纸被抠下来了）
            equippedIcon.sprite = null;
            equippedIcon.enabled = false;

            // 恢复空槽位的底图高亮
            if (backgroundImage != null) 
            {
                backgroundImage.color = new Color(1f, 1f, 1f, 1f); 
            }
        }
    }
}