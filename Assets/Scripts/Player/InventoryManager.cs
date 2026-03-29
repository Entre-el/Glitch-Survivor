using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    private List<WeaponSO> weaponSlots = new(6);
    [Header("Passive Item Slots")]
    private List<PassiveItemSO> passiveItemSlots = new(6);
    [Header("Weapon Levels")]
    private int[] weaponLevels = new int[6];
    private int[] passiveItemLevels = new int[6];
    public List<WeaponOrItemSO> upgradeOptions = new();
    private List<WeaponOrItemSO> PickedOptionSOList = new(4);
    private int maxUpgradeOptions = 4;
    private List<WeaponOrItemSO> availableUpgradeOptions = new();

    [Header("Audio")]
    public AudioClip upgradeSFX;
    private PlayerStats player;

    private void Start()
    {
        if(!TryGetComponent<PlayerStats>(out player))
        {
            Debug.LogError("PlayerStats component not found");
            return;
        }; 
        EventCenter.AddListener(EventDefine.OnLevelUpRequest, OnLevelUpRequest);
        EventCenter.AddListener<WeaponOrItemSO>(EventDefine.OnLevelUp,WeaponOrItemLevelUp);
    }

    private void OnLevelUpRequest()
    {
        PickUpgradeOptions();
    }
    private void WeaponOrItemLevelUp(WeaponOrItemSO chosenUpgrade)
    {
        if(chosenUpgrade.Type == WeaponOrItemType.Weapon)
        {
            WeaponSO weapon = chosenUpgrade as WeaponSO;
            for(int i = 0; i < weaponSlots.Count; i++)
            {
                if(weaponSlots[i] != null && weaponSlots[i].Name == weapon.Name)
                {
                    weaponLevels[i]++;
                    weaponSlots[i] = weapon;
                    return;
                }
            }
            weaponSlots.Add(weapon);
            weaponLevels[weaponSlots.Count - 1] = 1;
        }
        else if(chosenUpgrade.Type == WeaponOrItemType.Item)
        {
            PassiveItemSO passiveItem = chosenUpgrade as PassiveItemSO;
            for(int i = 0; i < passiveItemSlots.Count; i++)
            {
                if(passiveItemSlots[i] != null && passiveItemSlots[i].Name == passiveItem.Name)
                {
                    passiveItemLevels[i]++;
                    passiveItemSlots[i] = passiveItem;
                    return;
                }
            }
            passiveItemSlots.Add(passiveItem);
            passiveItemLevels[passiveItemSlots.Count - 1] = 1;
        }
    }
    private void PickUpgradeOptions()
    {
        PickedOptionSOList.Clear();
        availableUpgradeOptions.Clear();
        availableUpgradeOptions.AddRange(upgradeOptions);
        for(int i = 0; i < maxUpgradeOptions; i++)
        {
            if(availableUpgradeOptions.Count == 0) break;
            WeaponOrItemSO chosenUpgrade =PickUpgradeOption(availableUpgradeOptions);
            PickedOptionSOList.Add(chosenUpgrade);
        }
        EventCenter.Broadcast(EventDefine.OnOptionsPicked, PickedOptionSOList); 
    }
    private WeaponOrItemSO PickUpgradeOption(List<WeaponOrItemSO> availablePool)
    {
        int randomIndex = Random.Range(0, availablePool.Count);
        WeaponOrItemSO chosenOptionSO = availablePool[randomIndex];
        availablePool.RemoveAt(randomIndex);
        if (chosenOptionSO is null) return null;

        bool isNewWeapon = true;

        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null && weaponSlots[i].Name == chosenOptionSO.Name)
            {
                isNewWeapon = false;
                if(chosenOptionSO.NextLevelWeaponOrItemSO is null)
                {
                    return null;
                }
                int slotIndex = i;
                return chosenOptionSO.NextLevelWeaponOrItemSO;
            }
        }
        if(isNewWeapon)
        {
            return chosenOptionSO;
        }
        return null;
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<WeaponOrItemSO>(EventDefine.OnLevelUp, WeaponOrItemLevelUp);
    }
}
