using UnityEngine;

public enum EventDefine
{
    OnRequestSceneChange,
    OnPlayerDied,
    OnGameOver,
    OnGameWin,
    OnGameQuit,
    OnLevelUpRequest,
    OnResumeRequest,
    OnRestartRequest,
    OnQuitRequest,
    OnLoadingStart,
    OnLoadingScreenShown,
    OnLoadingScreenReady,
    OnPoolInit,
    OnLoadingScreenFinished,
    OnBossDied,
    OnExpChanged,
    OnLevelUp,
    OnWeaponLevelUp,
    OnOptionsPicked,
    OnHealthChanged,
    OnMaxHealthChanged,
    OnMoveSpeedChanged,
    OnRecoveryChanged,
    OnMightChanged,
    OnProjectileSpeedChanged,
    OnMagnetChanged,
    OnGameWon,
    OnActionPressed,
    OnActionReleased,
    OnMapPrepareRequst,
    OnPlayerDashed,
    OnPlayerDashEnd,
    OnChargeStart,
    OnPlayerExpChanged,
    OnPlayerLevelUp,
    OnDamagePopup,
    OnEnemyDied,
    OnBuffExpired,
}

// 定义伤害的语义类型
public enum DamageType
{
    Normal, // 普通伤害
    Critical, // 暴击
    Poison, // 毒属性持续伤害
    Heal, // 治疗
    Dodge, // 闪避（可飘字 "MISS"）
}

// 更新事件载荷
public struct DmgMessage
{
    public int amount;
    public Vector3 position;
    public DamageType damageType; // 取代原本的 bool isCritical
}
