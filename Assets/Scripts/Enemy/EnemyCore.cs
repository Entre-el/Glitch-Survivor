using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyLocomotion))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyVisuals))]
[RequireComponent(typeof(Transform))]
public class EnemyCore : MonoBehaviour
{
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
        EventCenter.AddListener(EventDefine.OnEnemyDied, OnEnemyDied);
    }

    public void AddBuff(BaseEnemyBuff newBuff, int stackCount = 1, float duration = 2f)
    {
        if (activeBuffs.Contains(newBuff))
        {
            newBuff.stackCount++; // 如果已经有这个 Buff 了，增加层数
            newBuff.OnAddStack(stackCount, duration); // 如果已经有这个 Buff 了，刷新持续时间
            return; // 已经有这个 Buff 了，直接返回
        }
        activeBuffs.Add(newBuff);
        newBuff.OnApply(); // 触发初始效果

        // 通知 UI 更新（使用我们之前学过的事件系统）
        // EventCenter.Broadcast(EventDefine.OnBuffAdded, myCore.gameObject, newBuff.buffData);
    }

    public void BuffTick()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            BaseEnemyBuff buff = activeBuffs[i];
            buff.OnTick(Time.deltaTime);

            // 检查 Buff 是否到期
            if (buff.timeRemaining <= 0)
            {
                buff.OnRemove();
                activeBuffs.RemoveAt(i);

                // 通知 UI 移除图标
                // EventCenter.Broadcast(EventDefine.OnBuffRemoved, myCore.gameObject, buff.buffData);
            }
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

    public void TakeDamage(float damage, bool isCrit)
    {
        Health.TakeDamage((int)damage);
        EventCenter.Broadcast(
            EventDefine.OnDamagePopup,
            new DmgMessage
            {
                amount = (int)damage,
                position = transform.position,
                isCritical = isCrit,
            }
        );
    }

    public void OnEnemyDied()
    {
        // 1. 播放死亡动画
        //Visuals.PlayDeathAnimation();

        // 2. 禁止敌人再动了
        Locomotion.enabled = false;

        // 4. 最后销毁对象（可以等动画播完）
        Destroy(gameObject, 1f); // 假设动画是1秒钟
    }
}
