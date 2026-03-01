using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private Transform player;
    public EnemyScriptableObject enemyData;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float  currentDamage;
    public float despawnDistance = 20f;
    void Start()
    {
        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;
    }
     void Awake()
    {
        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.MoveSpeed;
        currentDamage = enemyData.Damage;
    }
    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > despawnDistance)
            {
                ReturnEnemy();
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Kill();
        }
    }
    public void Kill()
    {
        GetComponent<DropRateManagerr>().DropItem();
        Destroy(gameObject);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
}
private void ReturnEnemy()
    {
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        transform.position = spawner.GetRoundEnemyPosition(spawner.roundEdgeLength);
    }
}