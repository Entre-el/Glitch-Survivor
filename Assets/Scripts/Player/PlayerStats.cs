using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public CharacterSO characterData;
    private float currentHealth;
    private float currentMaxHealth;
    private float currentMoveSpeed;
    private float currentRecovery;
    private float currentMight;
    private float currentProjectileSpeed;
    private float currentMagnet;
    public ParticleSystem damageEffect;
    #region Current Stats Properties
    private float CurrentHealth
    {
        get { return currentHealth; }
        set { if(currentHealth != value)
            {
                currentHealth = value;
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.currentHealthDisplay.text = "Health: " + currentHealth.ToString("F0");
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
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.currentHealthDisplay.text = "Health: " + currentHealth.ToString("F0") + "/" + currentMaxHealth.ToString("F0");
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
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed.ToString("F1");
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
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery.ToString("F1");
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
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.currentMightDisplay.text = "Might: " + currentMight.ToString("F1");
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
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed.ToString("F1");
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
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet.ToString("F1");
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
        public int experienceCap;
    } 
    InventoryManager inventory;
    public int nextWeaponIndex = 0;
    public int nextPassiveItemIndex = 0;
    [Header("UI")]
    Slider healthBar;
    public Image expBar;
    public Text  levelDisplay;
    [Header("Audio")]
    public AudioClip deathSFX;
    void Awake()
    {
        if(CharacterSelector.Instance != null)
        {
        characterData = CharacterSelector.GetData();
        }
        else if(characterData is null && CharacterSelector.Instance is null)
        {
           Debug.LogWarning("Character data not assigned and CharacterSelector Instance not found. Please assign characterData in the inspector or ensure CharacterSelector is set up correctly.");
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
        healthBar = GetComponentInChildren<Slider>();
        experienceCap = levelRanges[0].experienceCap;
        GameManager.Instance.currentHealthDisplay.text = "Health: " + CurrentHealth.ToString("F0") + "/" + CurrentMaxHealth.ToString("F0");
        GameManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed.ToString("F1");
        GameManager.Instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery.ToString("F1");
        GameManager.Instance.currentMightDisplay.text = "Might: " + CurrentMight.ToString("F1");
        GameManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed.ToString("F1");
        GameManager.Instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet.ToString("F1");
        GameManager.Instance.AssignChosenCharacterUI(characterData);
        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelDisplay();
    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        while (experience >= experienceCap)
        {
            experience -= experienceCap;
            EventCenter.Broadcast(EventDefine.OnLevelUp);
            level++;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCap = range.experienceCap;
                    break;
                }
            }
        }
        EventCenter.Broadcast(EventDefine.OnExpChanged, experience);
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
            if(damageEffect) Instantiate(damageEffect, transform.position, Quaternion.identity);
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
        GameManager.Instance.AssignLevelReachedUI(level);
        EventCenter.Broadcast(EventDefine.OnPlayerDied);
    }
    public void Win()
    {
        GameManager.Instance.AssignLevelReachedUI(level);
        EventCenter.Broadcast(EventDefine.OnGameWin);
    }
    public void RestoreHealth(float amount)
    {
        
        if (CurrentHealth < CurrentMaxHealth)
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
       
        GameObject newWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        newWeapon.transform.SetParent(transform);
        inventory = GetComponent<InventoryManager>();
        if (inventory != null)
        {    
            nextWeaponIndex++;
        }
    }
        public void SpawnPassiveItem(GameObject passiveItem)
    {
        
        GameObject newPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        newPassiveItem.transform.SetParent(transform);
        inventory = GetComponent<InventoryManager>();
       
    }
    void UpdateHealthBar()
    {
        if(healthBar != null)
        {
            healthBar.value = (float)CurrentHealth / (float)CurrentMaxHealth;
        }
    }
    void UpdateExpBar()
    {
        if(expBar != null)
        {
            expBar.fillAmount = (float)experience / (float)experienceCap;
        }
    }
    void UpdateLevelDisplay()
    {
        if(levelDisplay != null)
        {
            levelDisplay.text = "LV: " + level.ToString();
        }
    }
}
