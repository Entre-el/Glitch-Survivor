using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "ScriptableObjects/Weapon")]
public class WeaponSO : WeaponOrItemSO
{
    [SerializeField]
    new string name;
    public new string Name
    {
        get => name;
        private set => name = value;
    }

    [SerializeField]
    WeaponSO nextLevelWeaponSO;
    public WeaponSO NextLevelWeaponSO
    {
        get => nextLevelWeaponSO;
        private set => nextLevelWeaponSO = value;
    }

    [SerializeField]
    float damage;
    public float Damage
    {
        get => damage;
        private set => damage = value;
    }

    [SerializeField]
    float speed;
    public float Speed
    {
        get => speed;
        private set => speed = value;
    }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration
    {
        get => cooldownDuration;
        private set => cooldownDuration = value;
    }

    [SerializeField]
    int pierce;
    public int Pierce
    {
        get => pierce;
        private set => pierce = value;
    }

    [SerializeField]
    string description;
    public new string Description
    {
        get => description;
        private set => description = value;
    }

    [SerializeField]
    Sprite icon;
    public new Sprite Icon
    {
        get => icon;
        private set => icon = value;
    }
    private WeaponOrItemType type = WeaponOrItemType.Weapon;
    public new WeaponOrItemType Type
    {
        get => type;
        private set => type = value;
    }
}
