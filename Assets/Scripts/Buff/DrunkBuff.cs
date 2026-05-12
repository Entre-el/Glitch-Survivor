public class DrunkBuff : BaseEnemyBuff
{
    private float tickTimer = 0f;
    public override float SpeedMultiplier => 0.5f;

    // 缓存接口引用，避免每秒高频执行 RTTI 类型推断
    private IDamageable damageableTarget;

    public DrunkBuff(EnemyBuffSO data, IBuffable target, float? duration = null)
        : base(data, target, duration)
    {
        // 构造期执行一次模式匹配
        damageableTarget = target as IDamageable;
    }

    public override void OnTick(float deltaTime)
    {
        base.OnTick(deltaTime);
        tickTimer += deltaTime;

        if (tickTimer >= 1f)
        {
            tickTimer -= 1f;

            // 实例级防御性调用
            damageableTarget?.TakeDamage(5f, false, DamageType.Poison);
        }

        if (timeRemaining <= 0)
        {
            OnRemoveStack(1);
            timeRemaining = duration;
        }

        if (stackCount <= 0)
        {
            target.RemoveBuff(this);
        }
    }

    public override void OnAddStack(int addedStacks = 1, float? duration = null)
    {
        base.OnAddStack(addedStacks, duration ?? base.duration);
    }

    public override void OnRemoveStack(int removedStacks = 1)
    {
        base.OnRemoveStack(removedStacks);
    }
}
