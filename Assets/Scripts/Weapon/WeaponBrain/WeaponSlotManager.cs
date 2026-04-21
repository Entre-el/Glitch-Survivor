using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubWeaponContext
{
    public string ContextName; // "Main", "Left Hand", "Right Hand"
    public StickerSO PierceSticker;
    public StickerSO CritSticker;
    public StickerSO FadeSticker;
}

[System.Serializable]
public class WeaponSlotManager
{
    [Header("全武器共用：开火插槽")]
    public StickerSO FireSticker;

    [Header("后置插槽环境")]
    public List<SubWeaponContext> SubContexts = new List<SubWeaponContext>();

    // 🌟 完善：传入武器的基础伤害、基础穿透、基础击退
    public CombatPayload GeneratePayload(int contextIndex, float baseDmg, int basePierce, float baseKnockback, PlayerCore core)
    {
        SubWeaponContext ctx = SubContexts[contextIndex];
        StatCalculator stats = core.Stats; // 提取你的属性计算器
        
        // 🌟 核心机制 1：处理穿透 (Float 强转 Int)
        int finalPierce = basePierce + Mathf.FloorToInt(stats.PlayerPierce.Value);

        // 🌟 核心机制 2：如果有全局伤害倍率，可以在这里乘上 (假设你以后加了 PlayerDamageMultiplier)
        float finalDamage = baseDmg; // * stats.PlayerDamageMultiplier.Value;

        return new CombatPayload
        {
            // 基础面板组装
            FinalDamage = finalDamage,
            CritChance = stats.PlayerCritChance.Value,
            CritMultiplier = stats.PlayerCritMultiplier.Value,
            PierceCount = finalPierce,
            ProjectileSpeedMult = stats.PlayerProjectileSpeedMultiplier.Value,
            KnockbackForce = baseKnockback,
            BulletScale = 1f,

            // 贴纸绑定
            FireSticker = this.FireSticker,
            PierceSticker = ctx.PierceSticker,
            CritSticker = ctx.CritSticker,
            FadeSticker = ctx.FadeSticker,
            
            SourceEntity = core.gameObject
        };
    }
}