using System.Collections.Generic;
using UnityEngine;

public class PhysicsSimulationZone : MonoBehaviour
{
    private SimulationMode2D savedMode;
    private readonly List<Rigidbody2D> frozenWorldRbs = new();

    [Header("UI 专属重力")]
    public Vector2 uiGravity = new Vector2(0, -3000f);

    // 🌟 性能优化：缓存当前散落区内的所有贴纸刚体
    private readonly List<Rigidbody2D> activeStickers = new();

    // 标记是否需要重新搜集贴纸（脏标记）
    private bool needsCacheUpdate = false;

    void OnEnable()
    {
        savedMode = Physics2D.simulationMode;
        Physics2D.simulationMode = SimulationMode2D.Script;

        // 1. 冰封外部世界（仅执行一次，开销可控）
        frozenWorldRbs.Clear();
        Rigidbody2D[] allRbs = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
        foreach (var rb in allRbs)
        {
            if (!rb.transform.IsChildOf(this.transform) && rb.simulated)
            {
                rb.simulated = false;
                frozenWorldRbs.Add(rb);
            }
        }

        // 2. 标记需要更新缓存
        needsCacheUpdate = true;
    }

    void OnDisable()
    {
        Physics2D.simulationMode = savedMode;

        foreach (var rb in frozenWorldRbs)
        {
            if (rb != null)
                rb.simulated = true;
        }
        frozenWorldRbs.Clear();
        activeStickers.Clear();
    }

    // 🌟 当有新贴纸生成，或者有贴纸被销毁（装配到武器上）时，由外部调用此方法
    public void MarkCacheDirty()
    {
        needsCacheUpdate = true;
    }

    void Update()
    {
        if (Physics2D.simulationMode != SimulationMode2D.Script)
            return;

        float dt = Mathf.Min(Time.unscaledDeltaTime, 0.02f);

        // 🌟 性能优化：仅在必要时（刚打开背包，或有贴纸增减时）才重新获取组件
        if (needsCacheUpdate)
        {
            activeStickers.Clear();
            activeStickers.AddRange(GetComponentsInChildren<Rigidbody2D>(false));
            needsCacheUpdate = false;
        }

        // 🌟 极致丝滑的遍历：直接操作缓存列表，O(n) 开销极小
        for (int i = activeStickers.Count - 1; i >= 0; i--)
        {
            Rigidbody2D rb = activeStickers[i];

            // 安全防空：如果玩家在拖拽中途贴纸被销毁了，移出列表
            if (rb == null)
            {
                activeStickers.RemoveAt(i);
                continue;
            }

            // 施加重力
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.linearVelocity += uiGravity * dt;
            }
        }

        // 推进引擎
        Physics2D.Simulate(dt);
    }
}
