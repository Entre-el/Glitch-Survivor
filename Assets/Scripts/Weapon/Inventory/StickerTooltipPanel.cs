using UnityEngine;
using TMPro;
using System.Text;

public class StickerTooltipPanel : BasePanel
{
    public static StickerTooltipPanel Instance;

    [Header("Tooltip 引用")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public RectTransform rectTransform;

    public Vector2 offset = new Vector2(50f, -50f); 

    public override void OnInit()
    {
        base.OnInit();
        Instance = this;
        OnHide(); // 初始状态隐藏
    }

    // 🌟 核心修改：加入可为空的 StickerSlotType 参数
    public void ShowTooltip(StickerSO sticker, StickerSlotType? hoveredSlot = null)
    {
        if (!hasInit) OnInit();

        nameText.text = sticker.stickerName;

        // 状态 A：悬停在特定槽位上
        if (hoveredSlot.HasValue && hoveredSlot.Value != StickerSlotType.Any)
        {
            string specificDesc = sticker.GetDescriptionForSlot(hoveredSlot.Value);
            
            if (string.IsNullOrEmpty(specificDesc))
            {
                // 如果这个槽位没写描述，给个友好的提示
                descText.text = $"<color=#888888>该贴纸装配至【{hoveredSlot.Value}】槽位时无特殊效果。</color>";
            }
            else
            {
                // 重点突出当前槽位效果
                descText.text = $"<color=#FFD700><b>【{hoveredSlot.Value} 效果】</b></color>\n{specificDesc}";
            }
        }
        // 状态 B：在地上，或者悬空拖拽中（显示全部）
        else
        {
            StringBuilder sb = new StringBuilder();
            
            if (!string.IsNullOrEmpty(sticker.fireDescription)) 
                sb.AppendLine($"<b>【开火】</b> {sticker.fireDescription}");
                
            if (!string.IsNullOrEmpty(sticker.pierceDescription)) 
                sb.AppendLine($"<b>【穿透】</b> {sticker.pierceDescription}");
                
            if (!string.IsNullOrEmpty(sticker.critDescription)) 
                sb.AppendLine($"<b>【暴击】</b> {sticker.critDescription}");
                
            if (!string.IsNullOrEmpty(sticker.fadeDescription)) 
                sb.AppendLine($"<b>【消失】</b> {sticker.fadeDescription}");

            descText.text = sb.Length > 0 ? sb.ToString() : "暂无描述";
        }
        
        OnShow(); 
    }

    public void HideTooltip() { OnHide(); }
    private void Update()
    {
        // 🌟 通过判断 Alpha 值来决定是否需要跟随鼠标，避免了 activeSelf 的开销
        if (hasInit && canvasGroup.alpha > 0)
        {
            rectTransform.position = (Vector2)Input.mousePosition + offset;
        }
    }
}