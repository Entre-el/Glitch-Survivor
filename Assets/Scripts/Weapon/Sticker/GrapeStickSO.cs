using UnityEngine;

[CreateAssetMenu(menuName = "Stickers/Grape (葡萄)")]
public class GrapeStickerSO : StickerSO
{
    // 🍇 1. 开火槽：分裂为两个 0.6 倍伤害、0.8 倍大小的子弹
    public override void OnFireSlot(ref CombatPayload payload, Transform emitter, Vector2 direction)
    {
        SplitBullets(payload, emitter.position, direction, 2, 20f, 0.6f, 0.9f);
    }

    // 🍇 2. 穿透槽：穿透时分裂为两个 0.5 倍伤害、0.7 倍大小的子弹
    public override void OnPierceSlot(
        ref CombatPayload payload,
        IDamageable target,
        Vector3 hitPoint,
        Vector2 direction
    )
    {
        SplitBullets(payload, hitPoint, direction, 2, 20f, 0.5f, 0.8f, target);
    }

    // 🍇 3. 暴击槽：附加醉酒效果
    public override void OnCritSlot(ref CombatPayload payload, IDamageable target, Vector3 hitPoint)
    {
        if (target is IBuffable buffable)
        {
            buffable.AddBuff(new DrunkBuff(appliedBuffs[0], buffable)); // 直接应用醉酒状态，持续 2 秒
        }
    }

    // 🍇 4. 消失槽：留下一滩葡萄酒
    public override void OnFadeSlot(ref CombatPayload payload, Vector3 fadePoint)
    {
        if (puddlePrefab != null)
        {
            GameObject puddle = ObjectPoolManager.Instance.Get(
                puddlePrefab,
                fadePoint,
                Quaternion.identity
            );

            if (puddle != null)
            {
                // 🌟 核心：让生成的醉酒区域，继承这颗子弹临死前的尺寸！
                // 因为 Unity 的 CircleCollider2D 会自动随着 transform.localScale 缩放，
                // 所以碰撞判定范围会自动变小，完美符合你的需求！
                puddle.transform.localScale = new Vector3(
                    puddle.transform.localScale.x * payload.BulletScale,
                    puddle.transform.localScale.y * payload.BulletScale,
                    1f
                );
            }
        }
    }
}
