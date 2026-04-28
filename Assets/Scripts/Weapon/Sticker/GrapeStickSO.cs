using UnityEngine;

[CreateAssetMenu(menuName = "Stickers/Grape (葡萄)")]
public class GrapeStickerSO : StickerSO
{
    [Header("葡萄专属配置")]
    public GameObject grapeBulletPrefab; // 葡萄子弹预制体
    public GameObject winePuddlePrefab;  // 醉酒区域预制体
    public float splitAngle = 20f;       // 分裂角度

    // 🍇 1. 开火槽：分裂为两个 0.6 倍伤害、0.8 倍大小的子弹
    public override void OnFireSlot(CombatPayload payload, Transform emitter, Vector2 direction)
    {
        SplitBullets(payload, emitter.position, direction, 2, 20f, 0.6f, 0.9f);
    }

    // 🍇 2. 穿透槽：穿透时分裂为两个 0.5 倍伤害、0.7 倍大小的子弹
    public override void OnPierceSlot(CombatPayload payload, GameObject target, Vector3 hitPoint, Vector2 direction)
    {
        SplitBullets(payload, hitPoint, direction, 2, 20f, 0.5f, 0.8f, target.gameObject);
    }
    
    // 🍇 3. 暴击槽：附加醉酒效果
    public override void OnCritSlot( CombatPayload payload, GameObject target, Vector3 hitPoint)
    {
        if (target.TryGetComponent<EnemyCore>(out EnemyCore enemyCore))
        {
            enemyCore.AddBuff(new DrunkBuff(appliedBuffs[0],enemyCore)); // 直接应用醉酒状态，持续 2 秒
        }
    } 
// 🍇 4. 消失槽：留下一滩葡萄酒
    public override void OnFadeSlot(CombatPayload payload,Vector3 fadePoint)
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
    private void AddDrunkStack(EnemyCore core)
    {         
        // 给怪物添加一层醉酒状态，持续 2 秒
        // 你可以在 EnemyBuffSO 中定义一个 DrunkBuffSO，设置好持续时间和效果（如降低移动速度、攻击频率等）
        // 然后在这里实例化这个 Buff，并添加到怪物身上
    }
}