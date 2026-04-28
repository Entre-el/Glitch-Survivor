using UnityEngine;

// 记得挂上 PoolItem 方便回收
public class PuddleBase : PoolItem
{
    float duration = 3f; // 区域存在 3 秒
    private float timer;

    private void OnEnable()
    {
        timer = duration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReturnToPool();
        }
    }

    // 使用 OnTriggerStay2D，配合时间控制，实现“每秒附加一次”
    private float applyTimer;
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            applyTimer -= Time.deltaTime;
            if (applyTimer <= 0)
            {
                if (collision.TryGetComponent<EnemyHealth>(out var health))
                {
                    //health.AddDrunkStack();
                }
                applyTimer = 1f; // 冷却1秒后再次给踩在里面的怪上 Buff
            }
        }
    }
}