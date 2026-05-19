using System;
using System.Collections.Generic;
using TMPro; // 强烈依赖 TMP
using UnityEngine;

[CreateAssetMenu(fileName = "DamagePopupStyleConfig", menuName = "DamagePopupStyleConfig")]
public class DamagePopupStyleConfigSO : ScriptableObject
{
    [Serializable]
    public struct PopupStyle
    {
        public DamageType type;
        public Color textColor;
        public float sizeMultiplier; // 基础大小的缩放倍率
        public TMP_FontAsset customFont; // 可选的特殊字体
    }

    // 存储所有样式的列表
    [SerializeField]
    private List<PopupStyle> styles = new();

    // 供外部查询样式的方法
    public PopupStyle GetStyle(DamageType type)
    {
        foreach (var style in styles)
        {
            if (style.type == type)
                return style;
        }
        // 兜底返回默认样式
        return new PopupStyle { textColor = Color.white, sizeMultiplier = 1f };
    }
}
