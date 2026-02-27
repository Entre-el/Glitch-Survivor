using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentMaxHealth;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentMight;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;

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
    
    void Awake()
    {
        currentHealth = characterData.MaxHealth;
        currentMaxHealth = characterData.MaxHealth;
        currentMoveSpeed = characterData.MoveSpeed;
        currentRecovery = characterData.Recovery;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMagnet = characterData.Magnet;
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
            currentHealth -= damage;
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
            if (currentHealth <= 0)
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
        
        if (currentHealth > currentMaxHealth)
        {
            currentHealth += amount; 
        }
        else currentHealth = currentMaxHealth;
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
        if (currentHealth < currentMaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
            if (currentHealth > currentMaxHealth)
            {
                currentHealth = currentMaxHealth;
            }
        }
    }
}
