using UnityEngine;

[CreateAssetMenu(menuName = "Stickers/Pitaya (火龙果)")]
public class PitayaStickerSO : StickerSO
{
    // 1. 开火时:暴击率增加30%
    public override void OnFireSlot(CombatPayload payload, Transform emitter, Vector2 direction)
    {
        payload.CritChance += 30f; // 暴击率增加30%
    }

    // 2. 穿透时:子弹暴击率变为1.5倍
    public override void OnPierceSlot(
        CombatPayload payload,
        GameObject target,
        Vector3 hitPoint,
        Vector2 direction
    )
    {
        payload.CritChance *= 1.5f; // 子弹暴击率变为1.5倍
    }

    // 3. 暴击时:敌人下次受到的伤害翻倍
    public override void OnCritSlot(CombatPayload payload, GameObject target, Vector3 hitPoint)
    {
        if (target.TryGetComponent<EnemyCore>(out EnemyCore enemyCore))
        {
            enemyCore.AddBuff(new MarkedBuff(appliedBuffs[0], enemyCore, -1)); // 应用标记状态
        }
    }

    // 4. 消失时:生成一个区域对敌人施加脆弱(敌人受到暴击伤害+50%)
    public override void OnFadeSlot(CombatPayload payload, Vector3 fadePoint)
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
                puddle.transform.localScale = new Vector3(
                    puddle.transform.localScale.x * payload.BulletScale,
                    puddle.transform.localScale.y * payload.BulletScale,
                    1f
                );
            }
        }
    }
}
