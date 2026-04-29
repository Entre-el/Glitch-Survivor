using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshPro))]
public class DmgText : PoolItem
{
    private TMPro.TextMeshPro textMesh;

    // 🌟 核心：记录初始状态，防止对象池脏数据污染
    private float baseFontSize;
    private Color baseColor = Color.white;

    private void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshPro>();
        baseFontSize = textMesh.fontSize; // 记录预制体上配置的默认字号
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    // 修复了拼写错误 SetCiritical -> SetCritical
    public void SetCritical(bool isCritical)
    {
        if (isCritical)
        {
            textMesh.color = Color.red;
            textMesh.fontSize = baseFontSize * 1.5f; // 基于初始值放大，不会无限膨胀
        }
        else
        {
            textMesh.color = baseColor;
            textMesh.fontSize = baseFontSize; // 🌟 必须重置回默认大小！
        }
    }

    // 💡 必须要有回收逻辑！
    // 比如：你可以在生成后开启一个协程，或者用 DOTween 做动画，动画结束调用此方法
    public void RecycleAfterAnimation()
    {
        ReturnToPool();
    }
}
