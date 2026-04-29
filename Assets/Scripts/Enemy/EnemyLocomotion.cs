using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyLocomotion : MonoBehaviour
{
    private EnemyCore enemyCore;
    private TransformAnchorSO targetTransformSO;
    private Vector2 movementDirection;
    public float speed; // 基础移动速度
    Rigidbody2D rb;
    Vector2 knockbackVelocity;
    float knockbackDuration;

    public void Initialize()
    {
        TryGetComponent<EnemyCore>(out enemyCore); // 获取 EnemyCore 组件
        rb = GetComponent<Rigidbody2D>(); // 获取刚体
        if (enemyCore == null)
        {
            Debug.LogError("EnemyCore is not set");
            return;
        }
        speed = enemyCore.enemyStatSO.MoveSpeed; // 从 EnemyStatSO 获取移动速度
        targetTransformSO = enemyCore.TargetAnchor; // 从 EnemyCore 获取 TargetAnchor
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0)
            return;
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
