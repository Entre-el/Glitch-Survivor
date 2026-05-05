public class MarkedBuff : BaseEnemyBuff
{
    public MarkedBuff(EnemyBuffSO data, EnemyCore target, float duration = 5f)
        : base(data, target, duration) { }

    // 🌟 拦截即将到来的伤害
    public override float OnModifyDamage(float incomingDamage)
    {
        // 伤害翻倍
        float modifiedDamage = incomingDamage * 2f;

        OnRemoveStack(1);

        return modifiedDamage;
    }

    public override void OnAddStack(int addedStacks = 1, float? timeRemain = null)
    {
        stackCount += addedStacks;
    }

    public override void OnTick(float deltaTime) { }
}
