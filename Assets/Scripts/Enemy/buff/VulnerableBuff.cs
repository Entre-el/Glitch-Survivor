public class VulnerableBuff : BaseEnemyBuff
{
    public VulnerableBuff(EnemyBuffSO data, IBuffable target, float? duration = null)
        : base(data, target, duration) { }

    public override void OnApply() { }

    public override void OnTick(float deltaTime)
    {
        base.OnTick(deltaTime); // 别忘了扣总时间
        if (timeRemaining <= 0)
        {
            OnRemoveStack(1);
            timeRemaining = duration; // 刷新持续时间，直到所有层数都扣完
        }
        if (stackCount <= 0)
        {
            target.RemoveBuff(this); // 没有层数了，移除 Buff
        }
    }

    public override void OnAddStack(int addedStacks = 1, float? duration = null)
    {
        base.OnAddStack(addedStacks, duration ?? base.duration); // 叠加时刷新持续时间
    }

    public override void OnRemoveStack(int removedStacks = 1)
    {
        base.OnRemoveStack(removedStacks);
    }

    public override float OnModifyCirtDamage(float incomingDamage)
    {
        return incomingDamage * 1.5f;
    }
}
