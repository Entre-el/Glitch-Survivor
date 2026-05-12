using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BubbleProjectile : ProjectileBase
{
    [Header("子弹基础属性")]
    public new float currentSpeed = 10f;
    public new float lifeTime = 1f;

    public override void Initialize(
        CombatPayload payload,
        Vector2 direction,
        IDamageable ignoredTarget = null
    )
    {
        base.Initialize(payload, direction, ignoredTarget);
    }
}
