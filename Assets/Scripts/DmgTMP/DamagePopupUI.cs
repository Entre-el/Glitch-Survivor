using System.Collections;
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

        // 3. 应用视觉样式\
        if (message.isCirt)
        {
            textMesh.text += "!";
        }
        textMesh.color = style.textColor;
        textMesh.fontSize = baseFontSize * style.sizeMultiplier;
        if (style.customFont != null)
        {
            textMesh.font = style.customFont;
        }

        // 4. 执行动画（这里省略你原本的向上漂移、透明度渐变等动画代码）
        StartCoroutine(AnimateAndRecycle());
    }

    IEnumerator AnimateAndRecycle()
    {
        float duration = 1f; // 动画持续时间
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f; // 向上漂移

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 漂移
            transform.position = Vector3.Lerp(startPos, endPos, t);
            // 渐变（从完全不透明到完全透明）
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1 - t);

            yield return null;
        }

        // 动画结束后回收对象
        DmgPopupManager.CurrentActivePopups--;
        if (DmgPopupManager.CurrentActivePopups < 0)
            DmgPopupManager.CurrentActivePopups = 0;
        ReturnToPool();
    }
}
