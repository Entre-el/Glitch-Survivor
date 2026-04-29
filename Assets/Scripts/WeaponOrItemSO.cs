using UnityEngine;

public class WeaponOrItemSO : ScriptableObject
{
    [SerializeField]
    new string name;
    public string Name
    {
        get => name;
        private set => name = value;
    }

    [SerializeField]
    WeaponOrItemType type;
    public WeaponOrItemType Type
    {
        get => type;
        private set => type = value;
    }

    [SerializeField]
    string description;
    public string Description
    {
        get => description;
        private set => description = value;
    }

    [SerializeField]
    Sprite icon;
    public Sprite Icon
    {
        get => icon;
        private set => icon = value;
    }

    [SerializeField]
    WeaponOrItemSO nextLevelWeaponOrItemSO;
    public WeaponOrItemSO NextLevelWeaponOrItemSO
    {
        get => nextLevelWeaponOrItemSO;
        private set => nextLevelWeaponOrItemSO = value;
    }
}

public enum WeaponOrItemType
{
    Weapon,
    Item,
}
