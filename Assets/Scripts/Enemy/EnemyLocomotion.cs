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

    // 全局静态缓存, Zero GC
    private static readonly List<Collider2D> neighbors = new List<Collider2D>(16);
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

        // 1. Chasing Vector
        Vector2 chaseDir = (targetPosition - currentPosition).normalized;
        // 2. Separation Vector
        Vector2 sepForce = GetSeparationForce();

        // 3. Vector Blending & Normalize
        movementDirection = (chaseDir + sepForce).normalized;

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
}
