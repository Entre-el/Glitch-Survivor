using UnityEngine;

[CreateAssetMenu(menuName = "Stickers/Grape (葡萄)")]
public class GrapeStickerSO : StickerSO
{
    [Header("葡萄专属配置")]
    public GameObject grapeBulletPrefab; // 葡萄子弹预制体
    public GameObject winePuddlePrefab;  // 醉酒区域预制体
    public float splitAngle = 20f;       // 分裂角度

    // 🍇 1. 开火槽：分裂为两个 0.6 倍伤害、0.8 倍大小的子弹
    public override void OnFireSlot(Transform emitter, CombatPayload payload, Vector2 direction)
    {
        SplitBullets(emitter.position, payload, direction, 0.6f, 0.9f);
    }

    // 🍇 2. 穿透槽：穿透时分裂为两个 0.5 倍伤害、0.7 倍大小的子弹
    public override void OnPierceSlot(GameObject target, Vector3 hitPoint, CombatPayload payload, Vector2 direction)
    {
        SplitBullets(hitPoint, payload, direction, 0.5f, 0.8f, target);
    }

    // 🌟 修改通用分裂逻辑，接收 ignoredTarget
    // 🌟 修改通用分裂逻辑，增加一个 scaleMult 参数（尺寸缩小倍率）
    private void SplitBullets(Vector3 spawnPos, CombatPayload basePayload, Vector2 baseDirection, float damageMult, float scaleMult, GameObject ignoredTarget = null)
    {
        Vector2 dir1 = Quaternion.Euler(0, 0, splitAngle) * baseDirection;
        Vector2 dir2 = Quaternion.Euler(0, 0, -splitAngle) * baseDirection;

        CombatPayload newPayload = basePayload;
        newPayload.FinalDamage = basePayload.FinalDamage * damageMult;
        
        // 🌟 核心：子弹尺寸也要随之缩小！
        newPayload.BulletScale = basePayload.BulletScale * scaleMult;
        
        newPayload.PierceCount = Mathf.Max(0, basePayload.PierceCount - 1);
        if (newPayload.PierceCount <= 0) newPayload.PierceSticker = null; 

        SpawnBullet(spawnPos, newPayload, dir1, ignoredTarget);
        SpawnBullet(spawnPos, newPayload, dir2, ignoredTarget);
    }

    // 🌟 修改生成逻辑，注入 ignoredTarget
    private void SpawnBullet(Vector3 pos, CombatPayload payload, Vector2 dir, GameObject ignoredTarget)
    {
        GameObject bullet = ObjectPoolManager.Instance.Get(grapeBulletPrefab, pos, Quaternion.identity);
        if (bullet != null && bullet.TryGetComponent<ProjectileBase>(out var proj))
        {
            // 🌟 完美注入
            proj.Initialize(payload, dir, ignoredTarget);
        }
    }
    // 🍇 3. 暴击槽：附加醉酒效果
    public override void OnCritSlot(GameObject target, Vector3 hitPoint, CombatPayload payload)
    {
        if (target.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            // 给怪物挂上醉酒状态
            enemyHealth.AddDrunkStack();
        }
    }

// 🍇 4. 消失槽：留下一滩葡萄酒
    public override void OnFadeSlot(Vector3 fadePoint, CombatPayload payload)
    {
        if (winePuddlePrefab != null)
        {
            GameObject puddle = ObjectPoolManager.Instance.Get(winePuddlePrefab, fadePoint, Quaternion.identity);
            
            if (puddle != null)
            {
                // 🌟 核心：让生成的醉酒区域，继承这颗子弹临死前的尺寸！
                // 因为 Unity 的 CircleCollider2D 会自动随着 transform.localScale 缩放，
                // 所以碰撞判定范围会自动变小，完美符合你的需求！
                puddle.transform.localScale = new Vector3( puddle.transform.localScale.x*payload.BulletScale, puddle.transform.localScale.y* payload.BulletScale, 1f);
            }
        }
    }
}