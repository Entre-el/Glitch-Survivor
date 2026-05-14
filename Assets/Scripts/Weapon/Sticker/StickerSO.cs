using System.Collections.Generic;
using UnityEngine;

// 1. 定义四大生命周期插槽
public enum StickerSlotType
{
    Fire, // 开火槽（如：多重投射、散射）
    Pierce, // 穿透槽（如：穿透后分裂、弹射）
    Crit, // 暴击槽（如：暴击引发冰爆、吸血）
    Fade, // 消失槽（如：消失时留下毒素区域、爆炸）
    Any,
}

// 2. 贴纸数据资产基类 (ScriptableObject)
public abstract class StickerSO : ScriptableObject
{
    [Header("UI 展示数据")]
    public string stickerName;
    public Sprite icon;

    [Header("UI 槽位限制")]
    public StickerSlotType compatibleSlot = StickerSlotType.Any;

    // 🌟 新增：四个槽位的独立描述
    [Header("多槽位效果描述")]
    [TextArea]
    public string fireDescription;

    [TextArea]
    public string pierceDescription;

    [TextArea]
    public string critDescription;

    [TextArea]
    public string fadeDescription;

    [Header("涉及子弹及Buff")]
    public GameObject bulletPrefab; // 贴纸可能涉及的子弹预制体（如分裂后生成的子弹）
    public GameObject puddlePrefab; // 贴纸可能涉及的区域预制体（如消失后生成的毒 puddle）
    public List<EnemyBuffSO> appliedBuffs; // 贴纸可能附带的敌人 Buff 列表

    public string GetDescriptionForSlot(StickerSlotType slot)
    {
        return slot switch
        {
            StickerSlotType.Fire => fireDescription,
            StickerSlotType.Pierce => pierceDescription,
            StickerSlotType.Crit => critDescription,
            StickerSlotType.Fade => fadeDescription,
            _ => "",
        };
    }

    public virtual StatModifier[] GetStatModifiers()
    {
        return new StatModifier[0];
    }

    // 四大生命周期 Hook 虚方法 (由 DamageResolver 判官在对应时机回调)
    public virtual void OnFireSlot(ref CombatPayload payload, Transform emitter, Vector2 direction)
    {
        SpawnBullet(payload, emitter.position, direction, null);
    }

    public virtual void OnPierceSlot(
        ref CombatPayload payload,
        IDamageable target,
        Vector3 hitPoint,
        Vector2 direction
    ) { }

    public virtual void OnCritSlot(
        ref CombatPayload payload,
        IDamageable target,
        Vector3 hitPoint
    ) { }

    public virtual void OnFadeSlot(ref CombatPayload payload, Vector3 fadePoint) { }

    protected void SplitBullets(
        CombatPayload basePayload,
        Vector3 spawnPos,
        Vector2 baseDirection,
        int splitCount,
        float splitAngle,
        float damageMult,
        float scaleMult,
        IDamageable ignoredTarget = null
    )
    {
        CombatPayload newPayload = basePayload;
        newPayload.FinalDamage = basePayload.FinalDamage * damageMult;

        // 🌟 核心：子弹尺寸也要随之缩小！
        newPayload.BulletScale = basePayload.BulletScale * scaleMult;

        newPayload.PierceCount = Mathf.Max(0, basePayload.PierceCount - 1);
        if (newPayload.PierceCount <= 0)
            newPayload.PierceSticker = null;
        // ✅ 正确的分裂算法
        // 假设 splitAngle 是指子弹之间的夹角（比如 20度）
        float startAngle = -splitAngle * (splitCount - 1) / 2f;

        // i < splitCount，确保要2颗就只生成2颗
        for (int i = 0; i < splitCount; i++)
        {
            float angleOffset = startAngle + (i * splitAngle);
            Vector2 dir = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
            SpawnBullet(newPayload, spawnPos, dir, ignoredTarget);
        }
    }

    protected void SpawnBullet(
        CombatPayload payload,
        Vector3 pos,
        Vector2 dir,
        IDamageable ignoredTarget
    )
    {
        GameObject bullet = ObjectPoolManager.Instance.Get(bulletPrefab, pos, Quaternion.identity);
        if (bullet == null)
        {
            //Debug.LogError($"[{stickerName}] 无法生成子弹：贴纸和载荷中均未提供 bulletPrefab！");
            return;
        }
        else if (bullet.TryGetComponent<ProjectileBase>(out var proj))
        {
            // 🌟 完美注入
            proj.Initialize(payload, dir, ignoredTarget);
        }
    }
}
