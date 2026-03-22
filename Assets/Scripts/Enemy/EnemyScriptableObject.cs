using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField]
    string poolTag;
    public string PoolTag { get => PoolTag; private set => PoolTag = value; }
    public int defaultCapacity = 50;
    public int DefaultCapacity { get => defaultCapacity; private set => defaultCapacity = value; }
    public int maxSize = 200;
    public int MaxSize { get => maxSize; private set => maxSize = value; }
    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }
}