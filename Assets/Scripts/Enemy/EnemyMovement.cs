using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy; 
    PlayerMovement player;
    Transform playerTransform;
    
    Rigidbody2D rb;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>(); // 获取刚体
        
        player = GameObject.FindAnyObjectByType<PlayerMovement>();
        if(player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform != null && knockbackDuration <= 0)
        {
            if(transform.position.x > playerTransform.position.x + 0.2f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = -Mathf.Abs(currentScale.x);
                transform.localScale = currentScale;
            }
            else if(transform.position.x < playerTransform.position.x - 0.2f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x);
                transform.localScale = currentScale;
            }
        }
    }

    void FixedUpdate()
    {
        if(knockbackDuration > 0)
        {
            rb.linearVelocity = knockbackVelocity;
            
            knockbackDuration -= Time.fixedDeltaTime; 
        }
        else if(playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
                    
            rb.linearVelocity = direction * enemy.currentMoveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if(knockbackDuration > 0) return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}