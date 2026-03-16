using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponSlotImages = new List<Image>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public List<Image> passiveItemSlotImages = new List<Image>(6);
    public int[] passiveItemLevels = new int[6];

    [System.Serializable]
    public class WeaponUpgrade
    {
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public Text upgreadeNameDisplay;
        public Text upgradeDescriptionDisplay;
        public Image upgradeIconDisplay;
        public Button upgradeButton;
        [System.NonSerialized]
        public GameObject root;
        public void Init()
        {
            if (upgradeButton != null)
            root = upgradeButton.transform.parent.gameObject;
        }
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new();
    public List<UpgradeUI> upgradeOptionUIs = new();
    [Header("Audio")]
    public AudioClip upgradeSFX;
    private PlayerStats player;

    private void Start()
    {
        player = GetComponent<PlayerStats>();
        foreach (var ui in upgradeOptionUIs)
        {
            ui.Init();
        }
    }

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        if (slotIndex >= 0 && slotIndex < weaponSlots.Count)
        {
            weaponSlots[slotIndex] = weapon;
            weaponLevels[slotIndex] = weapon.weaponData.Level;
            weaponSlotImages[slotIndex].enabled = true;
            weaponSlotImages[slotIndex].sprite = weapon.weaponData.Icon;
            if(GameManager.instance != null && GameManager.instance.isLevelingUp)
            {
                GameManager.instance.EndLevelUp();
            }
        }
        else
        {
            Debug.LogError("Invalid weapon slot index: " + slotIndex);
        }
    }

    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        if (slotIndex >= 0 && slotIndex < passiveItemSlots.Count)
        {
            passiveItemSlots[slotIndex] = passiveItem;
            passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
            passiveItemSlotImages[slotIndex].enabled = true;
            passiveItemSlotImages[slotIndex].sprite = passiveItem.passiveItemData.Icon;

            if(GameManager.instance != null && GameManager.instance.isLevelingUp)
            {
                GameManager.instance.EndLevelUp();
            }
        }
        else
        {
            Debug.LogError("Invalid passive item slot index: " + slotIndex);
        }
    }

    public void LevelUpWeapon(int slotIndex, WeaponUpgrade targetUpgradeOption)
    {
        AudioManager.instance.PlaySFX(upgradeSFX,false);
        WeaponController weapon = weaponSlots[slotIndex];
        if (slotIndex >= 0 && slotIndex < weaponLevels.Length)
        {
            if(!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogWarning("Weapon at slot " + slotIndex + " is already at max level.");
                return;
            }
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;
            targetUpgradeOption.weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;
            if(GameManager.instance != null && GameManager.instance.isLevelingUp)
            {
                GameManager.instance.EndLevelUp();
            }
        }
        else
        {
            Debug.LogError("Invalid weapon slot index: " + slotIndex);
        }
    }

    public void LevelUpItem(int slotIndex, PassiveItemUpgrade targetUpgradeOption)
    {
        AudioManager.instance.PlaySFX(upgradeSFX,false);
        PassiveItem passiveItem = passiveItemSlots[slotIndex];
        if (slotIndex >= 0 && slotIndex < passiveItemLevels.Length)
        {
            if(!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogWarning("Passive item at slot " + slotIndex + " is already at max level.");
                return;
            }
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;
            targetUpgradeOption.passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;
            if(GameManager.instance != null && GameManager.instance.isLevelingUp)
            {
                GameManager.instance.EndLevelUp();
            }
        }
        else
        {
            Debug.LogError("Invalid passive item slot index: " + slotIndex);
        }
    }

    void ApplyUpgradeOption()
    {
        List<WeaponUpgrade> availableWeaponUpgrades = new(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new(passiveItemUpgradeOptions);

        foreach(var optionUI in upgradeOptionUIs)
        {
            int upgradeType = Random.Range(0, 2);
            if (upgradeType == 0 && availableWeaponUpgrades.Count == 0) upgradeType = 1;
            if (upgradeType == 1 && availablePassiveItemUpgrades.Count == 0) upgradeType = 0;
            
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0) 
            {
                optionUI.root.SetActive(false);
                continue;
            }

            if(upgradeType == 0)
            {
                ProcessWeaponUpgrade(optionUI, availableWeaponUpgrades);
            }
            else if(upgradeType == 1)
            {
                ProcessPassiveItemUpgrade(optionUI, availablePassiveItemUpgrades);
            }
        }
    }

    private void ProcessWeaponUpgrade(UpgradeUI uiNode, List<WeaponUpgrade> availablePool)
    {
        int randomIndex = Random.Range(0, availablePool.Count);
        WeaponUpgrade chosenUpgrade = availablePool[randomIndex];
        availablePool.RemoveAt(randomIndex);

        if (chosenUpgrade == null) return;

        bool isNewWeapon = true;

        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null && weaponSlots[i].weaponData == chosenUpgrade.weaponData)
            {
                isNewWeapon = false;
                if(!chosenUpgrade.weaponData.NextLevelPrefab)
                {
                    Debug.LogWarning("Weapon " + chosenUpgrade.weaponData.Name + " is already at max level.");
                    uiNode.root.SetActive(false);
                    return;
                }

                int slotIndex = i;
                uiNode.root.SetActive(true);
                uiNode.upgradeButton.onClick.AddListener(() => LevelUpWeapon(slotIndex, chosenUpgrade));
                var nextWeaponData = chosenUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData;
                BindDataToUI(uiNode, nextWeaponData.Name, nextWeaponData.Description, nextWeaponData.Icon);
                break;
            }
        }

        if(isNewWeapon)
        {
            uiNode.root.SetActive(true);
            uiNode.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenUpgrade.initialWeapon));
            BindDataToUI(uiNode, chosenUpgrade.weaponData.Name, chosenUpgrade.weaponData.Description, chosenUpgrade.weaponData.Icon);
        }
    }

    private void ProcessPassiveItemUpgrade(UpgradeUI uiNode, List<PassiveItemUpgrade> availablePool)
    {
        int randomIndex = Random.Range(0, availablePool.Count);
        PassiveItemUpgrade chosenUpgrade = availablePool[randomIndex];
        availablePool.RemoveAt(randomIndex);

        if (chosenUpgrade == null) return;

        bool isNewItem = true;

        for(int i = 0; i < passiveItemSlots.Count; i++)
        {
            if(passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenUpgrade.passiveItemData)
            {
                isNewItem = false;
                if(!chosenUpgrade.passiveItemData.NextLevelPrefab)
                {
                    uiNode.root.SetActive(false);
                    Debug.LogWarning("Passive item " + chosenUpgrade.passiveItemData.Name + " is already at max level.");
                    return;
                }
                int slotIndex = i;
                uiNode.root.SetActive(true);
                uiNode.upgradeButton.onClick.AddListener(() => LevelUpItem(slotIndex, chosenUpgrade));
                var nextItemData = chosenUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData;
                BindDataToUI(uiNode, nextItemData.Name, nextItemData.Description, nextItemData.Icon);
                break;
            }
        }

        if(isNewItem)
        {
            uiNode.root.SetActive(true);
            uiNode.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenUpgrade.initialPassiveItem));
            BindDataToUI(uiNode, chosenUpgrade.passiveItemData.Name, chosenUpgrade.passiveItemData.Description, chosenUpgrade.passiveItemData.Icon);
        }
    }

    private void BindDataToUI(UpgradeUI uiNode, string name, string description, Sprite icon)
    {
        uiNode.upgreadeNameDisplay.text = name;
        uiNode.upgradeDescriptionDisplay.text = description;
        uiNode.upgradeIconDisplay.sprite = icon;
    }

    void RemoveUpgradeOptions()
    {
        foreach(var optionUI in upgradeOptionUIs)
        {
            optionUI.upgradeButton.onClick.RemoveAllListeners();
        }
    }

    public void RemoveAndApplyUpgradeOptions()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOption();
    }
}
