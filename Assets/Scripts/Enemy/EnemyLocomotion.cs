using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLocomotion : MonoBehaviour
{
    private EnemyCore enemyCore;
    private TransformAnchorSO targetTransformSO;
    private Vector2 movementDirection;
    public float baseSpeed;
    public float currentSpeed;
    Rigidbody2D rb;
    Vector2 knockbackVelocity;
    float knockbackDuration;

    [Header("Swarm Separation")]
    public float separationRadius = 0.4f;
    public float separationWeight = 1.5f;
    public LayerMask enemyLayer;

    [Header("性能优化：降频扫描")]
    public float separationUpdateInterval = 0.15f; // 每 0.15 秒才扫描一次周围
    private float separationTimer;
    private Vector2 cachedSeparationForce = Vector2.zero; // 缓存上一次的斥力

    // 全局静态缓存, Zero GC
    private static readonly List<Collider2D> neighbors = new(16);
    private ContactFilter2D filter;

    public void Initialize(EnemyCore core)
    {
        enemyCore = core;
        rb = GetComponent<Rigidbody2D>();
        if (enemyCore == null)
            return;

        baseSpeed = enemyCore.enemyStatSO.MoveSpeed;
        targetTransformSO = enemyCore.TargetAnchor;

        // Init ContactFilter2D
        filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = enemyLayer,
        };
        separationTimer = Random.Range(0f, separationUpdateInterval);
    }

    public void FixedUpdate()
    {
        if (knockbackDuration > 0)
        {
            rb.linearVelocity = knockbackVelocity;
            knockbackDuration -= Time.fixedDeltaTime;
        }
        else
        {
            // 计时器轮询
            separationTimer -= Time.fixedDeltaTime;
            if (separationTimer <= 0f)
            {
                // 时间到了，重新扫描周围，并重置倒计时
                cachedSeparationForce = GetSeparationForce();
                separationTimer = separationUpdateInterval;
            }

            MoveTowardsTarget();
        }
    }

    public void MoveTowardsTarget()
    {
        if (targetTransformSO == null || targetTransformSO.Value == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 targetPosition = targetTransformSO.Value.position;
        Vector2 currentPosition = transform.position;

        Vector2 chaseDir = (targetPosition - currentPosition).normalized;

        // 🌟 直接使用缓存的斥力，不再每帧调用 GetSeparationForce()！
        movementDirection = (chaseDir + cachedSeparationForce).normalized;

        rb.linearVelocity = movementDirection * currentSpeed;
    }

    private Vector2 GetSeparationForce()
    {
        Vector2 force = Vector2.zero;
        // Broad-phase spatial query
        int count = Physics2D.OverlapCircle(
            transform.position,
            separationRadius,
            filter,
            neighbors
        );

        if (count <= 1)
            return force;

        for (int i = 0; i < count; i++)
        {
            if (neighbors[i].gameObject == gameObject)
                continue;

            Vector2 diff = (Vector2)transform.position - (Vector2)neighbors[i].transform.position;
            float sqrMag = diff.sqrMagnitude;

            if (sqrMag > 0 && sqrMag < separationRadius * separationRadius)
            {
                // Inverse-square law fallback
                force += diff.normalized / Mathf.Sqrt(sqrMag);
            }
        }

        return force.normalized * separationWeight;
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0)
            return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
#if UNITY_EDITOR
    // 仅编辑器模式下编译, 用于可视化排斥力场半径
    private void OnDrawGizmosSelected()
    {
        // 设置Gizmo颜色为半透明黄色
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        // 绘制线框圆，半径绑定 separationRadius
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
#endif
}
