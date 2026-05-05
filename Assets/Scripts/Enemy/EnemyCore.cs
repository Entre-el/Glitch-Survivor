using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyLocomotion))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyVisuals))]
[RequireComponent(typeof(Transform))]
public class EnemyCore : PoolItem, IDamageable, IBuffable
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1f);

    [field: SerializeField]
    public TransformAnchorSO TargetAnchor { get; private set; }
    public EnemyStatSO enemyStatSO;
    public EnemyLocomotion Locomotion { get; private set; }
    public EnemyHealth Health { get; private set; }
    public EnemyVisuals Visuals { get; private set; }
    private readonly List<BaseEnemyBuff> activeBuffs = new(4);

    private void Awake()
    {
        Locomotion = GetComponent<EnemyLocomotion>();
        Health = GetComponent<EnemyHealth>();
        Visuals = GetComponent<EnemyVisuals>();
        if (TargetAnchor == null)
        {
            TargetAnchor = FindAnyObjectByType<TransformAnchorSO>(); // 尝试在场景中找到一个 TransformAnchorSO 实例
            if (TargetAnchor == null)
            {
                Debug.LogError("TargetAnchor is not set");
                return;
            }
        }
        Initialize();
    }

    public void Initialize()
    {
        Locomotion.Initialize(this);
        Health.Initialize(this);
        Visuals.Initialize(this);
    }

    private void OnEnable()
    {
        Locomotion.enabled = true; // 唤醒植物人
        Locomotion.currentSpeed = Locomotion.baseSpeed; // 重置速度
        ClearBuffs(); // 确保从池子里拿出来的是干净的
    }

    // 🌟 新增：接入 Unity 的生命周期，推动 Buff 系统运行
    private void Update()
    {
        // 如果敌人死了（比如 Locomotion 被禁用了），就不再走 Buff 逻辑
        if (Locomotion != null && Locomotion.enabled)
        {
            BuffTick();
        }
    }

    //实现接口IBuffable的AddBuff方法
    public void AddBuff(BaseEnemyBuff newBuff, float duration = 2f, int stackCount = 1)
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.BuffType == newBuff.BuffType)
            {
                buff.OnAddStack(stackCount, duration);
                return; // 已经有这个 Buff 了，直接返回
            }
        }
        activeBuffs.Add(newBuff);
        newBuff.OnApply(); // 触发初始效果
        RecalculateSpeed(); // 叠加后重新计算速度
        // 通知 UI 更新（使用我们之前学过的事件系统）
        // EventCenter.Broadcast(EventDefine.OnBuffAdded, myCore.gameObject, newBuff.buffData);
    }

    public void RemoveBuff(BaseEnemyBuff buff)
    {
        if (activeBuffs.Remove(buff))
        {
            RecalculateSpeed(); // 移除后重新计算速度
            // 通知 UI 更新（使用我们之前学过的事件系统）
            // EventCenter.Broadcast(EventDefine.OnBuffRemoved, myCore.gameObject, buff.buffData);
        }
    }

    public void BuffTick()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            BaseEnemyBuff buff = activeBuffs[i];
            buff.OnTick(Time.deltaTime);
        }
    }

    public void ClearBuffs()
    {
        foreach (var buff in activeBuffs)
        {
            buff.OnRemove(); // 先触发清理逻辑
        }
        activeBuffs.Clear(); // 再清空列表
    }

    public void TakeDamage(float damage, bool isCrit, DamageType type, bool showPopup = true)
    {
        float finalDamage = damage;

        // 🌟 核心：伤害拦截管线。倒序遍历，让每个 Buff 都有机会修改最终伤害
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            finalDamage = activeBuffs[i].OnModifyDamage(finalDamage);
        }
        if (isCrit)
        {
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                finalDamage = activeBuffs[i].OnModifyCirtDamage(finalDamage);
            }
        }
        // 经过 Buff 修改后，真正扣减生命值
        Health.TakeDamage((int)finalDamage);

        if (showPopup)
        {
            EventCenter.Broadcast(
                EventDefine.OnDamagePopup,
                new DmgMessage
                {
                    amount = (int)finalDamage,
                    position = transform.position,
                    damageType = type, // 逻辑层只传递类型，不管颜色！
                }
            );
        }
    }

    public void OnDied()
    {
        // 1. 播放死亡动画
        //Visuals.PlayDeathAnimation();

        // 2. 禁止敌人再动了
        Locomotion.enabled = false;
        EventCenter.Broadcast(EventDefine.OnEnemyDied, this); // 广播一个事件，告诉系统这个敌人死了

        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return _waitForSeconds1; // 等待 1 秒钟，给动画时间播放
        ClearBuffs(); // 确保所有 Buff 都被清理掉
        // 3. 从对象池回收
        ReturnToPool();
    }

    private void RecalculateSpeed()
    {
        float minSlowMultiplier = 1f; // 记录最强减速（找最小值）
        float maxHasteMultiplier = 1f; // 记录最强加速（找最大值）

        foreach (var buff in activeBuffs)
        {
            float mult = buff.SpeedMultiplier;

            // 如果是减速 Buff (倍率小于 1)
            if (mult < 1f && mult < minSlowMultiplier)
            {
                minSlowMultiplier = mult;
            }
            // 如果是加速 Buff (倍率大于 1)
            else if (mult > 1f && mult > maxHasteMultiplier)
            {
                maxHasteMultiplier = mult;
            }
        }
        // 🌟 最终结算：基础速度 * 最强加速 * 最强减速
        // 例如：基础 10 * 加速 1.5 * 减速 0.5 = 最终速度 7.5
        Locomotion.currentSpeed = Locomotion.baseSpeed * maxHasteMultiplier * minSlowMultiplier;
    }
}
