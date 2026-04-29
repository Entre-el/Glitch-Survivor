using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBase : PoolItem
{
    private CombatPayload payload;
    private Rigidbody2D rb;

    [Header("子弹基础属性")]
    public float currentSpeed = 10f;
    public float lifeTime = 1.5f;
    private float timer;

    // 🌟 新增：记录这颗子弹不能打的敌人
    private GameObject ignoredTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 🌟 修改：加入 optional 的 ignoredTarget 参数
    public void Initialize(
        CombatPayload payload,
        Vector2 direction,
        GameObject ignoredTarget = null
    )
    {
        this.payload = payload;
        this.timer = lifeTime;
        this.ignoredTarget = ignoredTarget; // 存下来

        currentSpeed *= payload.ProjectileSpeedMult / 100f;
        rb.linearVelocity = direction.normalized * currentSpeed;
        transform.localScale = new Vector3(payload.BulletScale, payload.BulletScale, 1f);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            Die();
    }

    public void Die()
    {
        if (payload.FadeSticker != null)
        {
            payload.FadeSticker.OnFadeSlot(payload, transform.position);
        }
        ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            // 🌟 核心拦截：如果碰到的刚好是生出我的那个敌人，直接无视它！穿过去！
            if (collision.gameObject == ignoredTarget)
                return;

            Vector2 currentDirection = rb.linearVelocity.normalized;
            DamageResolver.ResolveCollision(
                collision.gameObject,
                payload,
                transform.position,
                currentDirection
            );

            if (payload.PierceCount > 0)
            {
                payload.PierceCount--;
                ignoredTarget = collision.gameObject;
            }
            else
            {
                Die();
            }
        }
        else if (collision.gameObject.layer == 8)
        {
            Die();
        }
    }
}
