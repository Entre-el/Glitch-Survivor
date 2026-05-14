using UnityEngine;

public class PitayaPuddle : BasePuddle
{
    protected override void ApplyBuffToEnemy(IBuffable enemy)
    {
        enemy.AddBuff(new VulnerableBuff(buffSO, enemy, duration)); // 给对象添加流血 Buff，持续时间使用默认值
    }
}
