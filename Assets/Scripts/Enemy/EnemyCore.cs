using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyLocomotion))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(EnemyVisuals))]
public class EnemyCore : PoolItem, IDamageable, IBuffable
{
    private static readonly WaitForSeconds _waitForSeconds03 = new(0.3f);

    [field: SerializeField]
    public TransformAnchorSO TargetAnchor { get; private set; }
    public EnemyStatSO enemyStatSO;
    public EnemyLocomotion Locomotion { get; private set; }
    public EnemyHealth Health { get; private set; }
    public EnemyVisuals Visuals { get; private set; }
    public EnemyBuffUIController BuffUIController { get; private set; }

    [SerializeField]
    private List<BaseEnemyBuff> activeBuffs = new(4);

    [Header("伤害合并优化")]
    private float accumulatedDamage = 0f; // 攒起来的总伤害
    private float damagePopupTimer = 0f; // 倒计时器
    private const float POPUP_DELAY = 0.1f; // 合并时间窗口 (0.1秒)
    private bool pendingPopup = false; // 是否有正在排队的飘字
    private bool hasCritInBatch = false; // 这批伤害里是否包含暴击
    private DamageType priorityDamageType = DamageType.Normal; // 优先显示的伤害类型

    //实现接口IDamageable的Getter方法
    public GameObject GameObject
    {
        get => gameObject;
    }
    public Transform Transform
    {
        get => transform;
    }

    private void Awake()
    {
        Locomotion = GetComponent<EnemyLocomotion>();
        Health = GetComponent<EnemyHealth>();
        Visuals = GetComponent<EnemyVisuals>();
        BuffUIController = GetComponentInChildren<EnemyBuffUIController>(); // 在子对象中寻找 UI 控制器
        if (TargetAnchor == null)
        {
            TargetAnchor = FindAnyObjectByType<TransformAnchorSO>(); // 尝试在场景中找到一个 TransformAnchorSO 实例
            if (TargetAnchor == null)
            {
                //Debug.LogError("TargetAnchor is not set");
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
        BuffUIController.Initialize();
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
        if (pendingPopup)
        {
            damagePopupTimer -= Time.deltaTime;
            if (damagePopupTimer <= 0f)
            {
                FlushDamagePopup(); // 时间到，弹出合并后的飘字！
            }
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
                UpdateBuffDisplay(); // 刷新层数
                return; // 已经有这个 Buff 了，直接返回
            }
        }
        activeBuffs.Add(newBuff);
        newBuff.OnApply(); // 触发初始效果
        RecalculateSpeed(); // 叠加后重新计算速度
        BuffUIController.UpdateBuffDisplay(activeBuffs);
    }

    public void UpdateBuffDisplay()
    {
        BuffUIController.UpdateBuffDisplay(activeBuffs);
    }

    public void RemoveBuff(BaseEnemyBuff buff)
    {
        if (activeBuffs.Remove(buff))
        {
            RecalculateSpeed(); // 移除后重新计算速度
            BuffUIController.UpdateBuffDisplay(activeBuffs);
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
        BuffUIController.ClearDisplay(); // 同步清理 UI 显示
    }

    public void TakeDamage(
        float damage,
        bool isCrit,
        DamageType type,
        Vector3? sourcePosition = null,
        float knockbackForce = 0f,
        bool showPopup = true
    )
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
        if (type == DamageType.Normal)
        {
            Visuals.PlayHitEffect();
            // 实体自决物理反馈
            if (knockbackForce > 0.001f && sourcePosition.HasValue)
            {
                Vector2 knockbackDir = (transform.position - sourcePosition.Value).normalized;

                // 假设实体拥有独立的运动控制器
                if (TryGetComponent<Rigidbody2D>(out var rb))
                {
                    // 可在此处乘上实体的抗性系数 (Mass / KnockbackResistance)
                    rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }

        if (showPopup)
        {
            accumulatedDamage += finalDamage;

            // 如果这批伤害里有任何一次是暴击，合并后的数字就显示为暴击字体
            if (isCrit)
                hasCritInBatch = true;

            // 如果有特殊伤害(如毒/火)，覆盖普通伤害颜色
            if (type != DamageType.Normal)
                priorityDamageType = type;

            if (!pendingPopup)
            {
                // 如果是这 0.1 秒内的第一发子弹，启动倒计时
                pendingPopup = true;
                damagePopupTimer = POPUP_DELAY;
            }
        }
        if (Health.currentHealth <= 0)
        {
            FlushDamagePopup(); // 先把最后的伤害飘字弹出来，再死掉
            OnDied();
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
        yield return _waitForSeconds03; // 等待 0.3 秒钟，给动画时间播放
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

    private void FlushDamagePopup()
    {
        if (!pendingPopup || accumulatedDamage <= 0)
            return;

        EventCenter.Broadcast(
            EventDefine.OnDamagePopup,
            new DmgMessage
            {
                amount = (int)accumulatedDamage,
                position = transform.position,
                damageType = priorityDamageType,
                isCirt = hasCritInBatch,
            }
        );

        // 状态重置，等待下一轮合并
        accumulatedDamage = 0f;
        pendingPopup = false;
        hasCritInBatch = false;
        priorityDamageType = DamageType.Normal;
    }
}
