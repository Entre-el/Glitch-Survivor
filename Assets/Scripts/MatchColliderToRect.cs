using UnityEngine;
using UnityEngine.EventSystems;

// 强制要求必须有这两个组件，防呆设计
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(BoxCollider2D))]
[ExecuteAlways] // 神奇标签：让这段代码在 Unity 编辑器模式下也能运行！
public class MatchColliderToRect : UIBehaviour // 继承 UIBehaviour 是为了监听 UI 尺寸变化
{
    private RectTransform rectTransform;
    private BoxCollider2D boxCollider;

    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider2D>();
        UpdateColliderSize();
    }

    // 🌟 核心魔法：每当 RectTransform 的尺寸发生变化时，Unity 会自动调用这个方法！
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        UpdateColliderSize();
    }

#if UNITY_EDITOR
    // 在编辑器里拖拽调节大小时也能实时预览
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateColliderSize();
    }
#endif

    private void UpdateColliderSize()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        if (rectTransform != null && boxCollider != null)
        {
            // 1. 同步大小：直接把 UI 的宽高赋给碰撞体
            boxCollider.size = rectTransform.rect.size;

            // 2. 同步偏移：极其重要！如果你的 UI 中心点（Pivot）不是 (0.5, 0.5)，碰撞体会偏掉。
            // rectTransform.rect.center 会完美计算出当前的物理中心偏移量。
            boxCollider.offset = rectTransform.rect.center;
        }
    }
}
