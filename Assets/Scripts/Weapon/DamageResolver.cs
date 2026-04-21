using UnityEngine;

public static class DamageResolver
{
    // 🌟 修复：新增入参 Vector2 projectileDirection，用来传递子弹当前的飞行方向
    public static void ResolveCollision(GameObject victim, CombatPayload payload, Vector3 hitPoint, Vector2 projectileDirection)
    {
        if (victim.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            bool isCrit = Random.value <= payload.CritChance;
            float actualDamage = payload.FinalDamage;

            if (isCrit)
            {
                // 这里建议用除以 100f，假设 CritMultiplier 是 150 代表 150%
                actualDamage *= (payload.CritMultiplier / 100f); 

                if (payload.CritSticker != null)
                {
                    payload.CritSticker.OnCritSlot(victim, hitPoint, payload);
                }
            }

            // 执行处决：扣血
            enemyHealth.TakeDamage(actualDamage, isCrit);

            // 处理物理击退
            if (payload.KnockbackForce > 0 && victim.TryGetComponent<Rigidbody2D>(out var enemyRb))
            {
                Vector2 knockbackDir = (victim.transform.position - payload.SourceEntity.transform.position).normalized;
                enemyRb.AddForce(knockbackDir * payload.KnockbackForce, ForceMode2D.Impulse);
            }

            // 🌟 核心劫持：触发【穿透时】插槽，并传入真实的子弹方向！
            if (payload.PierceSticker != null)
            {
                payload.PierceSticker.OnPierceSlot(victim, hitPoint, payload, projectileDirection); 
            }
        }
    }
    // 核心审判方法：传入受害者、战斗快照、以及击中的坐标
    public static void ResolveCollision(GameObject victim, CombatPayload payload, Vector3 hitPoint)
    {
        
        // 1. 甄别目标：只有身上带有生命值组件的才是合法受害者（这里假设你有一个 EnemyHealth 组件）
        if (victim.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            // 2. 暴击判定：掷骰子
            bool isCrit = Random.value <= payload.CritChance;
            float actualDamage = payload.FinalDamage;

            if (isCrit)
            {
                // 假设 CritMultiplier 是 150，代表 150% 暴击伤害
                actualDamage *= (payload.CritMultiplier / 100f); 

                // 🌟 核心劫持：触发【暴击时】插槽！
                if (payload.CritSticker != null)
                {
                    // 比如：暴击时吸血，或者暴击时引发冰爆
                    payload.CritSticker.OnCritSlot(victim, hitPoint, payload);
                }
            }

            // 3. 执行处决：扣血
            // 把是否暴击传给怪物，方便怪物头顶弹出红色的暴击伤害数字
            enemyHealth.TakeDamage(actualDamage, isCrit);

            // 4. 处理物理击退 (如果有需要的话)
            if (payload.KnockbackForce > 0 && victim.TryGetComponent<Rigidbody2D>(out var enemyRb))
            {
                Vector2 knockbackDir = (victim.transform.position - payload.SourceEntity.transform.position).normalized;
                // 给怪物一个瞬间的推力
                enemyRb.AddForce(knockbackDir * payload.KnockbackForce, ForceMode2D.Impulse);
            }

            // 5. 🌟 核心劫持：触发【穿透时】插槽！
            if (payload.PierceSticker != null)
            {
                // 比如：穿透时附带灼烧，或者穿透后分裂成两个子弹
                payload.PierceSticker.OnPierceSlot(victim, hitPoint, payload, Vector2.zero); // 这里暂时不传 direction，后续可以根据需要调整参数
            }
        }
    }
}