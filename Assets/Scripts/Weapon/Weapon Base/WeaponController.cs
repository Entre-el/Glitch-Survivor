using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject prefab;
    public float currentCooldown;
    protected PlayerMovement pm;
    public WeaponScriptableObject weaponData;

    // 初始化：读取武器数据并设置初始冷却
    protected virtual void Start()
    {
        pm = FindAnyObjectByType<PlayerMovement>();
        currentCooldown =  weaponData.CooldownDuration;
    }

    // 通过冷却计时器控制攻击频率
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0)
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration;
    } 
}