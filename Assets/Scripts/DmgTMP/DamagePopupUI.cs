using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class DamagePopupUI : PoolItem
{
    [SerializeField]
    private DamagePopupStyleConfigSO styleConfig;

    private TextMeshPro textMesh;
    private float baseFontSize;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        baseFontSize = textMesh.fontSize;
    }

    public void Setup(DmgMessage message)
    {
        var style = styleConfig.GetStyle(message.damageType);

        // 1. 纯靠视觉样式区分（颜色、字体、大小）
        textMesh.color = style.textColor;
        textMesh.alpha = 1f;
        textMesh.fontSize = baseFontSize * style.sizeMultiplier;

        if (style.customFont != null)
        {
            textMesh.font = style.customFont;
        }

        // 2. 纯粹的零 GC 文本渲染！
        if (message.damageType == DamageType.Dodge)
        {
            textMesh.SetText("MISS");
        }
        else
        {
            if (message.isCirt)
            {
                // 暴击伤害保留感叹号，使用官方提供的无 GC 格式化
                textMesh.SetText("{0}!", message.amount);
            }
            else
            {
                // 普通伤害纯数字，最干净、最极致的写法
                textMesh.SetText("{0}", message.amount);
            }
        }

        // 3. 执行动画
        StartCoroutine(AnimateAndRecycle());
    }

    IEnumerator AnimateAndRecycle()
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 漂移
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // 🌟 核心优化：直接修改 alpha 属性，避免 new Color 和全顶点颜色重建！
            textMesh.alpha = 1f - t;

            yield return null;
        }

        // 动画结束后回收对象
        DmgPopupManager.CurrentActivePopups--;
        if (DmgPopupManager.CurrentActivePopups < 0)
            DmgPopupManager.CurrentActivePopups = 0;

        ReturnToPool();
    }
}
