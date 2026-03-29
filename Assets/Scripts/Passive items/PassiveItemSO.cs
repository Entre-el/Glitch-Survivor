using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemSO", menuName = "ScriptableObjects/PassiveItem")]
public class PassiveItemSO : WeaponOrItemSO
{  
    [SerializeField] string name;
    public string Name { get => name; private set => name = value; }   [SerializeField] float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value; }
    [SerializeField] int level;
    public int Level { get => level; private set => level = value; }
    [SerializeField] string description;
    public string Description { get => description; private set => description = value; }
    [SerializeField] Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }
    private WeaponOrItemType type = WeaponOrItemType.Item;
    public WeaponOrItemType Type { get => type; private set => type = value; }
}
