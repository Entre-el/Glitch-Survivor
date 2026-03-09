using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;
    private float currentHealth;
    private float currentMaxHealth;
    private float currentMoveSpeed;
    private float currentRecovery;
    private float currentMight;
    private float currentProjectileSpeed;
    private float currentMagnet;
    #region Current Stats Properties
    private float CurrentHealth
    {
        get { return currentHealth; }
        set { if(currentHealth != value)
            {
                currentHealth = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth.ToString("F0");
                }
            }
        }
    }
    public float CurrentMaxHealth
    {
        get { return currentMaxHealth; }
        set { if(currentMaxHealth != value)
            {
                currentMaxHealth = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth.ToString("F0") + "/" + currentMaxHealth.ToString("F0");
                }
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set { if(currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed.ToString("F1");
                }
            }
        }
    }
    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set { if(currentRecovery != value)
            {
                currentRecovery = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery.ToString("F1");
                }
            }
        }
    }
    public float CurrentMight
    {
        get { return currentMight; }
        set { if(currentMight != value)
            {
                currentMight = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + currentMight.ToString("F1");
                }
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set { if(currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed.ToString("F1");
                }
            }
        }
    }
    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set { if(currentMagnet != value)
            {
                currentMagnet = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet.ToString("F1");
                }
            }
        }
    }
    #endregion

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    } 
    InventoryManager inventory;
    public int nextWeaponIndex = 0;
    public int nextPassiveItemIndex = 0;
    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public Text levelDisplay;
    void Awake()
    {
        if(characterData == null)
        {
        characterData = CharacterSelector.GetData();
        }
        CurrentHealth = characterData.MaxHealth;
        CurrentMaxHealth = characterData.MaxHealth;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentRecovery = characterData.Recovery;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;
        inventory = GetComponent<InventoryManager>();
        SpawnWeapon(characterData.StartingWeapon);
    }
    public List<LevelRange> levelRanges;
    void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;
        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth.ToString("F0") + "/" + CurrentMaxHealth.ToString("F0");
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed.ToString("F1");
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery.ToString("F1");
        GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight.ToString("F1");
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed.ToString("F1");
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet.ToString("F1");
        GameManager.instance.AssignChosenCharacterUI(characterData);
        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelDisplay();
    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
    }
    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            experience -= experienceCap;
            level++;
            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
            // 进入升级阶段：暂停游戏并刷新升级选项（武器/被动）
            GameManager.instance.StartLevelUp();
        }
    }
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible = false;
    public void TakeDamage(float damage)
    {
        if(!isInvincible)
        {
            CurrentHealth -= damage;
            UpdateHealthBar();
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }
    }
    public void Kill()
    {
        if(!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponUI(inventory.weaponSlotImages,inventory.passiveItemSlotImages);
            GameManager.instance.GameOver();
        }
    }
    public void RestoreHealth(float amount)
    {
        
        if (CurrentHealth > CurrentMaxHealth)
        {
            CurrentHealth += amount; 
        }
        else CurrentHealth = CurrentMaxHealth;
            UpdateHealthBar();
    }
    void Update()
    {
        if(invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
           isInvincible = false;
        }
        Recover();
    }
    void Recover()
    {
        if (CurrentHealth < CurrentMaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            if (CurrentHealth > CurrentMaxHealth)
            {
                CurrentHealth = CurrentMaxHealth;
            }
        }
    }
    public void SpawnWeapon(GameObject weapon)
    {
        if(nextWeaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogWarning("Maximum weapon slots reached. Cannot add more weapons.");
            return;
        }   
        GameObject newWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        newWeapon.transform.SetParent(transform);
        inventory = GetComponent<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddWeapon(nextWeaponIndex, newWeapon.GetComponent<WeaponController>());            
            nextWeaponIndex++;
        }
    }
        public void SpawnPassiveItem(GameObject passiveItem)
    {
        if(nextPassiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogWarning("Maximum passiveItem slots reached. Cannot add more passiveItems.");
            return;
        }   
        GameObject newPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        newPassiveItem.transform.SetParent(transform);
        inventory = GetComponent<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddPassiveItem(nextPassiveItemIndex, newPassiveItem.GetComponent<PassiveItem>());            
            nextPassiveItemIndex++;
        }
    }
    void UpdateHealthBar()
    {
        if(healthBar != null)
        {
            healthBar.fillAmount = (float)CurrentHealth / CurrentMaxHealth;
        }
    }
    void UpdateExpBar()
    {
        if(expBar != null)
        {
            expBar.fillAmount = (float)experience / experienceCap;
        }
    }
    void UpdateLevelDisplay()
    {
        if(levelDisplay != null)
        {
            levelDisplay.text = "Level: " + level.ToString();
        }
    }
}
