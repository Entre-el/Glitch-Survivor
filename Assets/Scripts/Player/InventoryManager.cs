using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    // 这里用“槽位数组 + 等级数组”的方式，避免在 UI/结算时反复从物体上取数据。
    public int[] weaponLevels = new int[6];
    // 武器槽位对应的 UI 图标（用于显示当前已持有武器）
    public List<Image> weaponSlotImages = new List<Image>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    // 被动物品槽位对应的 UI 图标（用于显示当前已持有被动物品）
    public List<Image> passiveItemSlotImages = new List<Image>(6);
    public int[] passiveItemLevels = new int[6];
    [System.Serializable]
    public class WeaponUpgrade
    {
        // 还没拥有该武器时，点击升级选项会生成它的“初始等级武器预制体”
        public GameObject initialWeapon;
        // 当前这条升级选项所指向的“当前等级数据”（升级后会被刷新为新等级的数据）
        public WeaponScriptableObject weaponData;
    }
    [System.Serializable]
    public class PassiveItemUpgrade
    {
        // 还没拥有该被动时，点击升级选项会生成它的“初始等级预制体”
        public GameObject initialPassiveItem;
        // 当前这条升级选项所指向的“当前等级数据”（升级后会被刷新为新等级的数据）
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
    public List<WeaponUpgrade> weaponUpgradeOptions = new ();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new ();
    public List<UpgradeUI> upgradeOptionUIs = new ();
    private PlayerStats player;
    private void Start()
    {
    player = GetComponent<PlayerStats>();

    foreach (var ui in upgradeOptionUIs)
    {
        ui.Init();
    }
    }
    public void AddWeapon(int slotIndex ,WeaponController weapon)
    {
        if (slotIndex >= 0 && slotIndex < weaponSlots.Count)
        {
            weaponSlots[slotIndex] = weapon;
            // 等级直接来自 ScriptableObject（每个等级一套数据）
            weaponLevels[slotIndex] = weapon.weaponData.Level;
            // 同步更新 UI 槽位图标
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
            // 同步更新 UI 槽位图标
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
    public void LevelUpWeapon(int slotIndex,WeaponUpgrade targetUpgradeOption)
    {
        WeaponController weapon = weaponSlots[slotIndex];
        if (slotIndex >= 0 && slotIndex < weaponLevels.Length)
        {
            if(!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogWarning("Weapon at slot " + slotIndex + " is already at max level.");
                return;
            }
            // 升级的实现方式：销毁旧等级武器，生成“下一等级预制体”，再塞回同一个槽位。
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform);
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;
            // 同一条“升级选项”在下次刷 UI 时应代表更高等级，因此这里同步更新它的 weaponData
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
    public void LevelUpItem(int slotIndex,PassiveItemUpgrade targetUpgradeOption)
    {
        PassiveItem passiveItem = passiveItemSlots[slotIndex];
        if (slotIndex >= 0 && slotIndex < passiveItemLevels.Length)
        {
            if(!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogWarning("Passive item at slot " + slotIndex + " is already at max level.");
                return;
            }
            // 被动物品升级同理：销毁旧等级，生成下一等级预制体，然后回填到原槽位。
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform);
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;
            // 同一条“升级选项”在下次刷 UI 时应代表更高等级，因此这里同步更新它的 passiveItemData
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
        // 内存拷贝：在堆内存中实例化两个新的 List，作为本轮 UI 抽取的“临时可用池”
        List<WeaponUpgrade> availableWeaponUpgrades = new(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new(passiveItemUpgradeOptions);

        foreach(var optionUI in upgradeOptionUIs)
        {
            optionUI.root.SetActive(true); // 先默认激活所有 UI 槽位，后续根据池子状态再决定是否隐藏
            // 状态机路由：决定当前 UI 槽位渲染哪种数据类型
            int upgradeType = Random.Range(0, 2); // 0：武器，1：被动
            
            // 耗尽降级逻辑：如果某一个池子的内存引用已被清空，强制将指令流导向另一个池子
            if (upgradeType == 0 && availableWeaponUpgrades.Count == 0) upgradeType = 1;
            if (upgradeType == 1 && availablePassiveItemUpgrades.Count == 0) upgradeType = 0;
            
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0) 
            {
                optionUI.root.SetActive(false);
                continue;
                 // 如果两个池子都空了，直接隐藏这个升级选项的按钮
            }

            // 根据路由分发到专属的内部处理函数，彻底解耦（压平了原本深达 5 层的代码缩进）
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

    // --- 辅助函数 1：专门处理武器的 UI 绑定与数据拆包 ---
    private void ProcessWeaponUpgrade(UpgradeUI uiNode, List<WeaponUpgrade> availablePool)
    {
        // 1. 随机寻址并移出池子，防止重复抽取
        int randomIndex = Random.Range(0, availablePool.Count);
        WeaponUpgrade chosenUpgrade = availablePool[randomIndex];
        availablePool.RemoveAt(randomIndex); // 使用 RemoveAt(index) 比 Remove(object) 在底层的内存偏移计算上更高效

        if (chosenUpgrade == null) return;

        bool isNewWeapon = true;

        // 2. 线性扫描：在当前已拥有的武器槽位内存数组中，比对 ScriptableObject 的指针引用
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            if(weaponSlots[i] != null && weaponSlots[i].weaponData == chosenUpgrade.weaponData)
            {
                isNewWeapon = false; // 指针比对命中，说明是升级分支

                if(!chosenUpgrade.weaponData.NextLevelPrefab)
                {
                    Debug.LogWarning("Weapon " + chosenUpgrade.weaponData.Name + " is already at max level.");
                    return; // 已经是满级，中断后续 UI 绑定
                }

                // 【底层闭包防坑关键】：这里必须将循环变量 i 拷贝到局部栈变量 slotIndex 中。
                // 否则 C# 编译器在生成匿名委托（Lambda闭包）的隐藏类时，会持有对引用变量 i 的指针。
                // 导致当玩家点击按钮时，i 的值永远是循环结束后的最终值，从而引发数组越界或逻辑错误。
                int slotIndex = i; 
                uiNode.upgradeButton.onClick.AddListener(() => LevelUpWeapon(slotIndex, chosenUpgrade));

                // 3. 数据拆包：通过反射/组件获取下一级的具体数据内存块，并渲染到 UI 顶点
                var nextWeaponData = chosenUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData;
                BindDataToUI(uiNode, nextWeaponData.Name, nextWeaponData.Description, nextWeaponData.Icon);
                break; // 命中后立即跳出线性扫描，节约 CPU 时钟周期
            }
        }

        // 4. 新增分支：如果没有在内存中找到匹配项，绑定实例化（Spawn）的新委托
        if(isNewWeapon)
        {
            uiNode.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenUpgrade.initialWeapon));
            BindDataToUI(uiNode, chosenUpgrade.weaponData.Name, chosenUpgrade.weaponData.Description, chosenUpgrade.weaponData.Icon);
        }
    }

    // --- 辅助函数 2：专门处理被动道具的 UI 绑定与数据拆包 ---
    // （底层逻辑与武器完全一致，拆分是为了符合单一职责原则 SRP）
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
                    Debug.LogWarning("Passive item " + chosenUpgrade.passiveItemData.Name + " is already at max level.");
                    return;
                }

                int slotIndex = i;
                uiNode.upgradeButton.onClick.AddListener(() => LevelUpItem(slotIndex, chosenUpgrade));

                var nextItemData = chosenUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData;
                BindDataToUI(uiNode, nextItemData.Name, nextItemData.Description, nextItemData.Icon);
                break;
            }
        }

        if(isNewItem)
        {
            uiNode.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenUpgrade.initialPassiveItem));
            BindDataToUI(uiNode, chosenUpgrade.passiveItemData.Name, chosenUpgrade.passiveItemData.Description, chosenUpgrade.passiveItemData.Icon);
        }
    }

    // --- 辅助函数 3：UI 渲染抽象 ---
    // 消除重复的 UI 赋值代码，将字符串指针和图像引用统一写入显存结构
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
            // 清除底层委托调用列表（InvocationList）中的所有指针引用，防止内存泄漏和重复触发
            optionUI.upgradeButton.onClick.RemoveAllListeners();
        }
    }

    public void RemoveAndApplyUpgradeOptions()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOption();
    }
}