using UnityEngine;

public static class DamageResolver
{
    // 🌟 修复：新增入参 Vector2 projectileDirection，用来传递子弹当前的飞行方向
    public static void ResolveCollision(
        IDamageable victim,
        ref CombatPayload payload,
        Vector3 hitPoint,
        Vector2 projectileDirection
    )
    {
        bool isCrit = Random.Range(0f, 100f) <= payload.CritChance;
        float actualDamage = payload.FinalDamage;
        if (isCrit)
        {
            // 假设 CritMultiplier 是 150，代表 150% 暴击伤害
            actualDamage *= payload.CritMultiplier / 100f;
        }

        victim.TakeDamage(
            (int)actualDamage,
            isCrit,
            DamageType.Normal,
            hitPoint,
            payload.KnockbackForce,
            true
        );
        // 🌟 核心劫持：触发【暴击时】插槽！
        if (payload.CritSticker != null)
        {
            // 比如：暴击时吸血，或者暴击时引发冰爆
            payload.CritSticker.OnCritSlot(ref payload, victim, hitPoint);
        }

        // 4. 🌟 核心劫持：触发【穿透时】插槽！
        if (payload.PierceSticker != null && payload.PierceCount > 0)
        {
            // 比如：穿透时附带灼烧，或者穿透后分裂成两个子弹
            payload.PierceSticker.OnPierceSlot(ref payload, victim, hitPoint, projectileDirection);
        }
    }
}

// 3. 战斗载荷/快照 (纯数据结构体，极为轻量)
[System.Serializable]
public struct CombatPayload
{
    public float FinalDamage;
    public float CritChance;
    public float CritMultiplier;
    public int PierceCount;
    public float ProjectileSpeedMult;
    public float KnockbackForce;
    public float BulletScale;
    public Vector2 OriginalDirection;

    // 四大插槽贴纸
    public StickerSO FireSticker;
    public StickerSO PierceSticker;
    public StickerSO CritSticker;
    public StickerSO FadeSticker;

    public GameObject SourceEntity;
}
