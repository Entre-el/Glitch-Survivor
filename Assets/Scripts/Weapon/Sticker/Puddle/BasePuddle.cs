using System.Collections.Generic;
using UnityEngine;

public class BasePuddle : PoolItem
{
    public EnemyBuffSO buffSO;
    public float duration = 3f;
    public float buffApplyInterval = 1f;

    [Header("Spatial Query")]
    public float radius = 2f;
    public LayerMask targetLayer;

    protected float timeRemaining;
    protected float applyTimer;

    // 预分配容量，避免扩容导致GC；复用实例
    protected static readonly List<Collider2D> hitBufferList = new(128);

    // 物理查询过滤器，Struct类型
    protected ContactFilter2D contactFilter;

    protected virtual void Awake()
    {
        // 初始化Filter配置
        contactFilter = new ContactFilter2D
        {
            useTriggers = true, // 视敌人Collider配置而定
            useLayerMask = true,
            layerMask = targetLayer,
        };
    }

    protected virtual void OnEnable()
    {
        timeRemaining = duration;
        applyTimer = buffApplyInterval;
    }

    protected virtual void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            ReturnToPool();
            return;
        }

        applyTimer += Time.deltaTime;
        if (applyTimer >= buffApplyInterval)
        {
            applyTimer = 0f;
            ApplyAoEEffect();
        }
    }

    protected virtual void ApplyAoEEffect()
    {
        // 引擎底层会自动调用 hitBufferList.Clear()
        int hitCount = Physics2D.OverlapCircle(
            transform.position,
            radius,
            contactFilter,
            hitBufferList
        );

        for (int i = 0; i < hitCount; i++)
        {
            if (hitBufferList[i].TryGetComponent(out IBuffable buffable))
            {
                ApplyBuffToEnemy(buffable);
            }
        }
    }

    protected virtual void ApplyBuffToEnemy(IBuffable enemy)
    {
        // 交由子类重写
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 0.8f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
