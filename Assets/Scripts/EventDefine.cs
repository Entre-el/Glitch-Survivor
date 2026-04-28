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
}

public struct DmgMessage
{
    public int amount;
    public Vector3 position;
    public bool isCritical;
    public GameObject attacker;
}
