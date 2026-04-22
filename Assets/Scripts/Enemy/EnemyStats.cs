using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyLocomotion))]
public class EnemyStats : MonoBehaviour
{
    private Transform playerTransform;
    public EnemyStatSO enemyData;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;
    public float despawnDistance = 20f;
    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    [Header("Audio")]
    public AudioClip hitSFX;
    Color originalColor;
    SpriteRenderer sr;
    EnemyLocomotion enemyMovement;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        enemyMovement = GetComponent<EnemyLocomotion>();
    }
    void Awake()
    {
        currentHealth = enemyData.MaxHealth;
        currentMoveSpeed = enemyData.MoveSpeed;
        currentDamage = enemyData.Damage;
    }
    void Update()
    {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > despawnDistance)
            {
                ReturnEnemy();
            }
    }
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        AudioManager.Instance.PlayHitSFX(hitSFX);
        StartCoroutine(DamageFlash());
        if (knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            enemyMovement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }
        if (dmg > 0) GameManager.GenerateDamageText(Mathf.FloorToInt(dmg), transform);
        if (currentHealth <= 0)
        {
            Kill();
        }
    }
    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSecondsRealtime(damageFlashDuration);
        sr.color = originalColor;
    }
    private bool isDead = false;
    public void Kill()
    {
        if (isDead) return;
        isDead = true;
        GetComponent<DropRateManager>().DropItem();
        StartCoroutine(KillFade());
        EnemySpawner.Instance.OnEnemyKilled();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
        }
    }
    private void ReturnEnemy()
    {
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        transform.position = spawner.GetRoundEnemyPosition(spawner.roundEdgeLength);
    }
    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }
        Destroy(gameObject);
    }
}