using UnityEngine;

public class PitayaPuddle : BasePuddle
{
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IBuffable>(out IBuffable buffable)) // 只对实现了 IBuffable 接口的对象应用效果
        {
            buffable.AddBuff(new VulnerableBuff(buffSO, buffable, defaultDuration)); // 给对象添加流血 Buff，持续时间使用默认值
        }
    }
}
