// 🌟 纯 C# 类，极其轻量！不产生任何 GC 和组件开销
using UnityEngine;

public abstract class BaseEnemyBuff
{
    public EnemyBuffSO buffData; // 引用的数据
    protected float defaultDuration = 2f;
    public float duration = 2f; // 持续时间
    public float timeRemaining; // 剩余时间
    protected IBuffable target; // 宿主
    public int stackCount = 0;
    public BuffType BuffType => buffData.buffType; // 方便外部访问 Buff 类型
    public virtual float SpeedMultiplier => 1f; // 这个属性可以被其他 Buff 叠加时访问，返回一个减速倍率，默认为 1（不变）

    // 构造函数：初始化 Buff
    public BaseEnemyBuff(EnemyBuffSO data, IBuffable target, float? duration = null)
    {
        this.buffData = data;
        this.duration = duration ?? defaultDuration;
        this.timeRemaining = duration ?? defaultDuration;
        this.target = target;
    }

    // 1. 刚挂上时的瞬间效果
    public virtual void OnApply()
    {
        timeRemaining = duration; // 刷新持续时间
    }

    public virtual void OnTick(float deltaTime)
    {
        timeRemaining -= deltaTime;
        if (timeRemaining <= 0)
        {
            OnRemoveStack(1);
            if (stackCount > 0)
            {
                timeRemaining = duration;
            }
            else
            {
                // 没有层数了，通知宿主移除我
                target.RemoveBuff(this);
            }
        }
        if (stackCount <= 0)
        {
            target.RemoveBuff(this); // 没有层数了，移除 Buff
        }
    }

    public virtual float OnModifyDamage(float incomingDamage)
    {
        // 默认不修改伤害，直接返回原值
        return incomingDamage;
    }

    public virtual float OnModifyCirtDamage(float incomingDamage)
    {
        // 默认不修改伤害，直接返回原值
        return incomingDamage;
    }

    public virtual void OnAddStack(int addedStacks = 1, float? timeRemain = null)
    {
        stackCount += addedStacks;
        timeRemaining = timeRemain ?? duration; // 刷新持续时间
    }

    public virtual void OnRemoveStack(int removedStacks = 1)
    {
        stackCount = Mathf.Max(0, stackCount - removedStacks);
    }

    // 3. 结束时的清理工作（比如恢复移速）
    public virtual void OnRemove() { }
}
