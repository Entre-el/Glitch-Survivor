using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // 1. 本地抓取同物体的核心引用，拒绝单例
    private PlayerCore core;
    private float currentHealth;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => core.Stats.PlayerMaxHealth.Value;

    public void Initialize(PlayerCore core)
    {
        this.core = core;
        
        // 2. 把原本在 Start 里的初始化逻辑也搬过来，这样时序绝对安全
        currentHealth = MaxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // 3. 防跌破 0
        currentHealth = Mathf.Max(currentHealth, 0);
        
        // 4. 修正广播：只传枚举，不传参数。让监听了此事件的 UI 自己来读取 CurrentHealth 和 MaxHealth
        EventCenter.Broadcast(EventDefine.OnHealthChanged);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        
        // 5. 防溢出：确保加血不会超过当前的 MaxHealth
        currentHealth = Mathf.Min(currentHealth, MaxHealth);
        
        EventCenter.Broadcast(EventDefine.OnHealthChanged);
    }

    private void Die()
    {
        EventCenter.Broadcast(EventDefine.OnPlayerDied);
        // 执行死亡相关的物理或动画逻辑
    }
}