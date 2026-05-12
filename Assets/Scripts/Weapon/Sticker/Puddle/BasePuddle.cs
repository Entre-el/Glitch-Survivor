using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BasePuddle : PoolItem
{
    public EnemyBuffSO buffSO; // 这个可以在编辑器里赋值，或者通过代码加载
    protected float defaultDuration = 3f;
    protected float defaultBuffApplyInterval = 1f;
    public float duration = 3f; // 区域存在 3 秒
    public float buffApplyInterval = 1f; // 给踩在里面的怪上 Buff 的频率，单位秒
    protected float timeRemaining;
    protected float applyTimer; // 用于控制给踩在里面的怪上 Buff 的频率

    private void OnEnable()
    {
        timeRemaining = duration;
        applyTimer = 0f;
    }

    private void Update()
    {
        timeRemaining -= Time.deltaTime;
        applyTimer += Time.deltaTime;
        if (timeRemaining <= 0)
        {
            ReturnToPool();
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IBuffable buffable)) // 只对实现了 IBuffable 接口的对象应用效果
        {
            if (applyTimer > defaultBuffApplyInterval)
            {
                buffable.AddBuff(new VulnerableBuff(buffSO, buffable, defaultDuration)); // 给对象添加流血 Buff，持续时间使用默认值
                applyTimer = 0;
            }
        }
    }
}
