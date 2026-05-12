using System.Collections.Generic;
using UnityEngine;

// 加上这个标签，你在 Scene 视图里拖拽怪物，就能实时看到遮挡变化，不用点 Play！
[ExecuteAlways]
public class YSorter : MonoBehaviour
{
    [Header("主渲染器 (怪物的身体)")]
    public SpriteRenderer[] mainRenderers;

    [Header("全局排序设置")]
    [Tooltip("如果是固定的树木/石头，勾选此项节省性能")]
    public bool isStatic = false;
    public int sortOffset = 0; // 整体图层偏移量

    [System.Serializable]
    public struct ChildRenderer
    {
        public SpriteRenderer renderer;
        public int relativeOrder; // 相对排序（如：+20 代表永远在身体上面）
    }

    [Header("静态附属渲染器 (预制体自带的阴影、武器等)")]
    public List<ChildRenderer> staticChildRenderers = new List<ChildRenderer>();

    // 存放运行时动态生成的渲染器（如：被攻击后才冒出来的 Buff 图标）
    private List<ChildRenderer> dynamicChildRenderers = new List<ChildRenderer>();

    // 缓存当前计算出的基础 Order
    private int currentBaseOrder;

    private void Update()
    {
        // 如果是静态物体，运行后只排一次序就休眠
        if (Application.isPlaying && isStatic)
        {
            UpdateSorting();
            enabled = false;
            return;
        }

        UpdateSorting();
    }

    private void UpdateSorting()
    {
        // 🌟 核心引擎：Y坐标乘以 -100。
        // 为什么是负数？因为屏幕越靠下，Y值越小，计算出的 Order 就越大，就会遮挡上面的东西。
        currentBaseOrder = Mathf.RoundToInt(-transform.position.y * 100) + sortOffset;

        // 1. 同步怪物的身体
        foreach (var r in mainRenderers)
        {
            if (r != null)
                r.sortingOrder = currentBaseOrder;
        }

        // 2. 同步静态子物体（如阴影）
        foreach (var child in staticChildRenderers)
        {
            if (child.renderer != null)
                child.renderer.sortingOrder = currentBaseOrder + child.relativeOrder;
        }

        // 3. 同步动态注册的 UI（如 Buff 图标）
        foreach (var child in dynamicChildRenderers)
        {
            if (child.renderer != null)
                child.renderer.sortingOrder = currentBaseOrder + child.relativeOrder;
        }
    }

    // 🌟 给 Buff 控制器留的后门：动态注册
    public void RegisterDynamicRenderer(SpriteRenderer renderer, int relativeOrder)
    {
        if (renderer == null)
            return;
        dynamicChildRenderers.Add(
            new ChildRenderer { renderer = renderer, relativeOrder = relativeOrder }
        );
        renderer.sortingOrder = currentBaseOrder + relativeOrder; // 注册瞬间立刻排一次，防止闪烁
    }
}
