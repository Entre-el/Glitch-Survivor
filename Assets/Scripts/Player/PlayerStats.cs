using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

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
            }
        }
    }
    public float CurrentMaxHealth
    {
        get { return currentMaxHealth; }
        set { if(currentMaxHealth != value)
            {
                currentMaxHealth = value;
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set { if(currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
            }
        }
    }
    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set { if(currentRecovery != value)
            {
                currentRecovery = value;
            }
        }
    }
    public float CurrentMight
    {
        get { return currentMight; }
        set { if(currentMight != value)
            {
                currentMight = value;
            }
        }
    }
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set { if(currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
            }
        }
    }
    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set { if(currentMagnet != value)
            {
                currentMagnet = value;
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
    public GameObject firstPassiveItem;
    public GameObject secondPassiveItem;
    public GameObject SecondaryWeapon;
    
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
        SpawnWeapon(SecondaryWeapon);
        SpawnPassiveItem(firstPassiveItem); 
        SpawnPassiveItem(secondPassiveItem);
    }
    public List<LevelRange> levelRanges;
    void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;
    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
    }
    void LevelUpChecker()
    {
        while (experience >= experienceCap)
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
        Debug.Log("PLAYER IS DEAD");
    }
    public void RestoreHealth(float amount)
    {
        
        if (CurrentHealth > CurrentMaxHealth)
        {
            CurrentHealth += amount; 
        }
        else CurrentHealth = CurrentMaxHealth;
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
}
