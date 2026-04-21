using System.Collections.Generic;
using UnityEngine;

// 1. 定义四大生命周期插槽
public enum StickerSlotType
{
    Fire,   // 开火槽（如：多重投射、散射）
    Pierce, // 穿透槽（如：穿透后分裂、弹射）
    Crit,   // 暴击槽（如：暴击引发冰爆、吸血）
    Fade    // 消失槽（如：消失时留下毒素区域、爆炸）
}

// 2. 贴纸数据资产基类 (ScriptableObject)
public abstract class StickerSO : ScriptableObject
{
    [Header("贴纸基础信息")]
    public string stickerName;
    public List<string> description = new(4);
    public Sprite icon;
    
    // 留给子类实现：当贴纸被装备时，提供哪些数值修饰？
    // 比如：返回一组 StatModifier 列表
    public virtual StatModifier[] GetStatModifiers() { return new StatModifier[0]; }

    // 四大生命周期 Hook 虚方法 (由 DamageResolver 判官在对应时机回调)
    public virtual void OnFireSlot(Transform emitter, CombatPayload payload, Vector2 direction) { }
    public virtual void OnPierceSlot(GameObject target, Vector3 hitPoint, CombatPayload payload, Vector2 direction) { }
    public virtual void OnCritSlot(GameObject target, Vector3 hitPoint, CombatPayload payload) { }
    public virtual void OnFadeSlot(Vector3 fadePoint, CombatPayload payload) { }
}

// 3. 战斗载荷/快照 (纯数据结构体，极为轻量)
public struct CombatPayload
{
    public float FinalDamage;
    public float CritChance;
    public float CritMultiplier;      // 🌟 新增：暴击伤害倍率
    public int PierceCount;           // 🌟 新增：向下取整后的穿透次数
    public float ProjectileSpeedMult; // 🌟 新增：弹速倍率（子弹生成时读取）
    public float KnockbackForce;      // 击退力
    public float BulletScale;

    // 四大插槽贴纸
    public StickerSO FireSticker;
    public StickerSO PierceSticker;
    public StickerSO CritSticker;
    public StickerSO FadeSticker;

    public GameObject SourceEntity; 
}