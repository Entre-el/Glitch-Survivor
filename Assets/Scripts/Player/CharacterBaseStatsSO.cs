using UnityEngine;
[CreateAssetMenu(menuName = "Stats/Character Base Stats")]
public class CharacterBaseStatsSO :ScriptableObject
{
    [SerializeField]
    private float playerMoveSpeed = 5f;
    public float PlayerMoveSpeed { get => playerMoveSpeed; private set => playerMoveSpeed = value; }
    [SerializeField]
    private float playerCritChance = 0f;
    public float PlayerCritChance { get => playerCritChance; private set => playerCritChance = value; }
    [SerializeField]
    private float playerCritMultiplier = 100f;
    public float PlayerCritMultiplier { get => playerCritMultiplier; private set => playerCritMultiplier = value; }
    [SerializeField]
    private float playerPierce =0f;
    public float PlayerPierce { get => playerPierce; private set => playerPierce = value; }
    [SerializeField]
    private float playerMagnetRadius = 3f;
    public float PlayerMagnetRadius { get => playerMagnetRadius; private set => playerMagnetRadius = value; }
    [SerializeField]
    private float playerProjectileSpeedMultiplier =100f;
    public float PlayerProjectileSpeedMultiplier { get => playerProjectileSpeedMultiplier; private set => playerProjectileSpeedMultiplier = value; }
    [SerializeField]
    private float playerRecoveryPre5s = 0f;
    public float PlayerRecoveryPre5s { get => playerRecoveryPre5s; private set => playerRecoveryPre5s = value; }
    [SerializeField]
    private float playerMaxHealth = 100f;
    public float PlayerMaxHealth { get => playerMaxHealth; private set => playerMaxHealth = value; }
    [SerializeField]
    private float playerDashSpeed = 20f;
    public float PlayerDashSpeed { get => playerDashSpeed; private set => playerDashSpeed = value; }
    [SerializeField]
    private float playerDashCooldown = 1f;
    public float PlayerDashCooldown { get => playerDashCooldown; private set => playerDashCooldown = value; }
}