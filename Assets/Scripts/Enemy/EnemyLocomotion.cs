using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLocomotion : MonoBehaviour
{
    EnemyBrain enemyCore; 
    Transform targetTransform;
    public TransformAnchorSO TargetAnchor; // 通过 Inspector 赋值
    Rigidbody2D rb;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    void Start()
    {
        TryGetComponent<EnemyBrain>(out enemyCore); // 获取 EnemyBrain 组件
        rb = GetComponent<Rigidbody2D>(); // 获取刚体
        if(enemyCore == null)
        {
            Debug.LogError("EnemyBrain is not set");
            return;
        }
        targetTransform = enemyCore.TargetAnchor.Value;
    }

    void Update()
    {
        if (targetTransform != null && knockbackDuration <= 0)
        {
            if(transform.position.x > targetTransform.position.x + 0.2f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = -Mathf.Abs(currentScale.x);
                transform.localScale = currentScale;
            }
            else if(transform.position.x < targetTransform.position.x - 0.2f)
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
        else if(targetTransform != null)
        {
            Vector2 direction = (targetTransform.position - transform.position).normalized;
                    
            rb.linearVelocity = direction * enemyCore.Stats.currentMoveSpeed;
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