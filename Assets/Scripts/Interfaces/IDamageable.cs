using UnityEngine;

public interface IDamageable
{
    GameObject GameObject { get; }
    Transform Transform { get; }

    // 引入 sourcePosition 与 knockbackForce
    void TakeDamage(
        float damage,
        bool isCrit,
        DamageType type,
        Vector3? sourcePosition = null,
        float knockbackForce = 0f,
        bool showPopup = true
    );
}
