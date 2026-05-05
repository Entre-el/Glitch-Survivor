using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLocomotion : MonoBehaviour
{
    private EnemyCore enemyCore;
    private TransformAnchorSO targetTransformSO;
    private Vector2 movementDirection;
    public float baseSpeed; // 基础移动速度
    public float currentSpeed; // 当前移动速度，考虑 Buff 的影响
    Rigidbody2D rb;
    Vector2 knockbackVelocity;
    float knockbackDuration;

    public void Initialize(EnemyCore core)
    {
        enemyCore = core;
        rb = GetComponent<Rigidbody2D>(); // 获取刚体
        if (enemyCore == null)
        {
            Debug.LogError("EnemyCore is not set");
            return;
        }
        baseSpeed = enemyCore.enemyStatSO.MoveSpeed; // 从 EnemyStatSO 获取移动速度
        targetTransformSO = enemyCore.TargetAnchor; // 从 EnemyCore 获取 TargetAnchor
    }

    public void FixedUpdate()
    {
        if (knockbackDuration > 0)
        {
            rb.linearVelocity = knockbackVelocity; // 施加击退速度
            knockbackDuration -= Time.fixedDeltaTime; // 减少击退持续时间
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    public void MoveTowardsTarget()
    {
        if (targetTransformSO == null || targetTransformSO.Value == null)
            return;

        Vector2 targetPosition = targetTransformSO.Value.position;
        Vector2 currentPosition = transform.position;
        movementDirection = (targetPosition - currentPosition).normalized; // 计算朝向目标的方向
        rb.linearVelocity = movementDirection * currentSpeed; // 应用移动速度
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0)
            return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
