public class DrunkBuff : BaseEnemyBuff
{
    private float originalSpeed;
    private float tickTimer = 0f;

    public DrunkBuff(EnemyBuffSO data, EnemyCore target, float duration = 2f)
        : base(data, target, duration) { }

    // 1. 刚挂上时：处理减速
    public override void OnApply()
    {
        originalSpeed = target.Locomotion.speed; // 先记住原始速度
        target.Locomotion.speed *= 0.5f; // 中毒了，走路慢半拍！
    }

    // 2. 持续期间：处理持续扣血（每秒扣一次）
    public override void OnTick(float deltaTime)
    {
        base.OnTick(deltaTime); // 别忘了扣总时间

        tickTimer += deltaTime;
        if (tickTimer >= 1f)
        {
            tickTimer -= 1f;
            target.Health.TakeDamage(5); // 每秒扣 5 点血
        }
    }

    // 3. 结束时：恢复速度
    public override void OnRemove()
    {
        target.Locomotion.speed = originalSpeed;
    }
}
