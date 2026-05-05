using UnityEngine;

// 任何可受击的实体必须实现此接口
public interface IDamageable
{
    // showPopup 默认值为 true，不影响以前写的攻击代码
    void TakeDamage(float damage, bool isCrit, DamageType type, bool showPopup = true);
}
