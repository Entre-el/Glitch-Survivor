using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : MonoBehaviour
{
    private EnemyCore myCore;
    public int maxHealth = 20;
    public int currentHealth = 20;

    public void Initialize(EnemyCore core)
    {
        myCore = core;
        maxHealth = myCore.enemyStatSO.MaxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = math.min(currentHealth + healAmount, maxHealth);
    }

    public void Die()
    {
        EventCenter.Broadcast(EventDefine.OnEnemyDied, myCore);
    }
}
