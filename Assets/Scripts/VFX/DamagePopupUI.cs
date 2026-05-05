using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class DamagePopupUI : PoolItem
{
    [SerializeField]
    private DamagePopupStyleConfigSO styleConfig; // 拖入你创建的 SO 配置文件

    private TextMeshPro textMesh;
    private float baseFontSize;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        baseFontSize = textMesh.fontSize;
    }

    // 当接收到事件，从对象池取出发起调用时
    public void Setup(DmgMessage message)
    {
        // 1. 获取对应配置
        var style = styleConfig.GetStyle(message.damageType);

        // 2. 应用数值与文本
        if (message.damageType == DamageType.Dodge)
        {
            textMesh.text = "MISS";
        }
        else
        {
            textMesh.text = $"{style.prefix}{message.amount}";
        }

        // 3. 应用视觉样式
        textMesh.color = style.textColor;
        textMesh.fontSize = baseFontSize * style.sizeMultiplier;
        if (style.customFont != null)
        {
            textMesh.font = style.customFont;
        }

        // 4. 执行动画（这里省略你原本的向上漂移、透明度渐变等动画代码）
        // ...
    }
}
