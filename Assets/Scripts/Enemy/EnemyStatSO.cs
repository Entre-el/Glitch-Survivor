using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatSO", menuName = "SO/EnemyStatSO")]
public class EnemyStatSO : ScriptableObject
{
    [SerializeField]
    private float moveSpeed = 3f;
    public float MoveSpeed
    {
        get => moveSpeed;
        private set => moveSpeed = value;
    }

    [SerializeField]
    private int maxHealth = 10;
    public int MaxHealth
    {
        get => maxHealth;
        private set => maxHealth = value;
    }

    [SerializeField]
    private float damage;
    public float Damage
    {
        get => damage;
        private set => damage = value;
    }

    [SerializeField]
    private int denfense;
    public int Denfense
    {
        get => denfense;
        private set => denfense = value;
    }
}
