using System.Collections;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyHealth : MonoBehaviour
{
    private EnemyCore myCore;
    public float maxHealth = 20f;
    public float currentHealth = 20f;

    public void Initialize(EnemyCore core)
    {
        myCore = core;
        maxHealth = myCore.enemyStatSO.MaxHealth;
        currentHealth = maxHealth;
    }
}
