using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private EnemyCore core;

    private void Awake()
    {
        TryGetComponent<EnemyCore>(out EnemyCore enemyCore);
        core = enemyCore;
    }

    public void Attack()
    {
        // 🌟 核心：直接调用核心组件里的数据和方法！不需要再 GetComponent 了！
        if (core.TargetAnchor == null)
        {
            Debug.LogWarning("EnemyAttack: TargetAnchor is not set.");
            return;
        }

        // 🌟 这里你可以根据需要添加攻击逻辑，比如：
        // - 计算伤害
        // - 播放攻击动画
        // - 触发攻击特效
        // - 检测是否命中玩家并造成伤害
    }
}
