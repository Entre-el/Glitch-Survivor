using UnityEngine;

public class StatCalculator
{
    [Header("角色核心面板")]
    public readonly CharacterStat PlayerMoveSpeed;
    public readonly CharacterStat PlayerCritChance;
    public readonly CharacterStat PlayerCritMultiplier;
    public readonly CharacterStat PlayerPierce;
    public readonly CharacterStat PlayerMagnetRadius;
    public readonly CharacterStat PlayerProjectileSpeedMultiplier;
    public readonly CharacterStat PlayerRecoveryPre5s;
    public readonly CharacterStat PlayerMaxHealth;
    public readonly CharacterStat PlayerDashSpeed;
    public readonly CharacterStat PlayerDashCooldown;

    public StatCalculator(CharacterBaseStatsSO baseStats)
    {
        PlayerMoveSpeed = new CharacterStat(baseStats.PlayerMoveSpeed);
        PlayerCritChance = new CharacterStat(baseStats.PlayerCritChance);
        PlayerCritMultiplier = new CharacterStat(baseStats.PlayerCritMultiplier);
        PlayerPierce = new CharacterStat(baseStats.PlayerPierce);
        PlayerMagnetRadius = new CharacterStat(baseStats.PlayerMagnetRadius);
        PlayerProjectileSpeedMultiplier = new CharacterStat(
            baseStats.PlayerProjectileSpeedMultiplier
        );
        PlayerRecoveryPre5s = new CharacterStat(baseStats.PlayerRecoveryPre5s);
        PlayerMaxHealth = new CharacterStat(baseStats.PlayerMaxHealth);
        PlayerDashSpeed = new CharacterStat(baseStats.PlayerDashSpeed);
        PlayerDashCooldown = new CharacterStat(baseStats.PlayerDashCooldown);
    }

    // 你之前写的特定方法，适合快速调用
    public void AddSpeedModifier(float modifier)
    {
        PlayerMoveSpeed.AddModifier(new StatModifier(modifier, StatModType.PercentAdd, this));
    }

    // 🌟 架构建议：对于晶片系统，以后你会遇到成百上千种状态修改。
    // 你可以不写死 specific 的方法，而是让晶片直接访问 public readonly 的变量，例如：
    // core.Stats.PlayerPierce.AddModifier(new StatModifier(1f, StatModType.Flat, this));
}
