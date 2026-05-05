using UnityEngine;

public class GrapePuddle : BasePuddle
{
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision); // 先调用基类的逻辑，处理时间和频率控制

        if (collision.TryGetComponent<EnemyCore>(out EnemyCore enemyCore)) // 只对敌人应用效果
        {
            applyTimer = 0f; // 重置计时器
            if (enemyCore != null)
            {
                enemyCore.AddBuff(new DrunkBuff(buffSO, enemyCore, defaultDuration)); // 给敌人添加醉酒 Buff，持续时间使用默认值
            }
        }
    }
}
