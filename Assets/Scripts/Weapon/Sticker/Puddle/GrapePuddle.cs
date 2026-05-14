using UnityEngine;

public class GrapePuddle : BasePuddle
{
    protected override void ApplyBuffToEnemy(IBuffable enemy)
    {
        enemy.AddBuff(new MarkedBuff(buffSO, enemy, duration)); // 给对象添加虚弱 Buff，持续时间使用默认值
    }
}
