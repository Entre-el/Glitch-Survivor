using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterStat
{
    public float BaseValue;

    // 存储所有的修饰器
    protected readonly List<StatModifier> statModifiers;

    // 脏标记与缓存值
    protected bool isDirty = true;
    protected float lastBaseValue = float.MinValue;
    protected float _value; // 缓存的最终计算结果

    public CharacterStat(float baseValue)
    {
        BaseValue = baseValue;
        statModifiers = new List<StatModifier>(8);
    }

    // 外界高频调用时，极速返回缓存值，没有任何 foreach！
    public virtual float Value
    {
        get
        {
            if (isDirty || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false; // 清除脏标记
            }
            return _value;
        }
    }

    public virtual void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        // 按照计算顺序进行排序 (Flat -> PercentAdd -> PercentMult)
        statModifiers.Sort(CompareModifierOrder);
    }

    public virtual void AddModifiers(float Value, StatModType Type, int Order, object Source)
    {
        AddModifier(new StatModifier(Value, Type, Order, Source));
    }

    // 究极绝招：根据“来源”安全移除！再也不怕同数值误删了
    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        // 倒序遍历安全删除
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    private int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; // 如果顺序一样，保持原样
    }

    // 核心算法：如何混合固定值和百分比
    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0; // 用来累加 PercentAdd 类型的总和

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == StatModType.PercentAdd)
            {
                // 注意：百分比加法是相互叠加的。例如两个 +10%，等于 +20%
                sumPercentAdd += mod.Value;

                // 如果这是最后一个 PercentAdd，或者下一个修饰器不是 PercentAdd 了，就结算一次
                if (
                    i + 1 >= statModifiers.Count
                    || statModifiers[i + 1].Type != StatModType.PercentAdd
                )
                {
                    finalValue *= (1.0f + sumPercentAdd);
                    sumPercentAdd = 0;
                }
            }
            else if (mod.Type == StatModType.PercentMult)
            {
                // 百分比乘法是独立相乘的（极为强力的“更多”伤害）
                finalValue *= (1.0f + mod.Value);
            }
        }

        // 可以根据需要返回 Mathf.Round() 取整，这里保留浮点精度
        return (float)Math.Round(finalValue, 4);
    }
}

// 定义修饰器的类型（数值越大，计算优先级越靠后）
public enum StatModType
{
    Flat = 100, // 固定值加减 (如：攻击力 +5)
    PercentAdd = 200, // 百分比加法 (如：攻击力 +10%, 多个同类相互叠加)
    PercentMult = 300, // 百分比独立乘法 (如：最终伤害 x1.5, 极为稀有和强大的词缀)
}
