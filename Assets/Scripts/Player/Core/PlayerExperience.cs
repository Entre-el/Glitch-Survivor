using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    private PlayerCore core;

    [Header("等级数据")]
    public int CurrentLevel { get; private set; } = 1;
    public float CurrentExp { get; private set; } = 0f;
    public float ExpToNextLevel { get; private set; } = 10f; // 第一级升第二级需要10点经验

    [Header("经验曲线配置")]
    public float expMultiplierPerLevel = 1.2f; // 每升一级，所需经验变成上一级的 1.2 倍
    public float flatExpIncreasePerLevel = 5f; // 每升一级，额外增加 5 点基础需求

    public void Initialize(PlayerCore core)
    {
        this.core = core;
    }

    // 提供给外部调用的加经验接口（比如玩家吃到了经验球）
    public void AddExp(float amount)
    {
        CurrentExp += amount;
        while (CurrentExp >= ExpToNextLevel)
        {
            LevelUp();
        }

        // 每次加经验都广播一下，方便 UI 层更新屏幕顶部的经验条进度
        float expProgress = CurrentExp / ExpToNextLevel;
        EventCenter.Broadcast(EventDefine.OnPlayerExpChanged, expProgress);
    }

    private void LevelUp()
    {
        // 扣除当前级别的需求经验，溢出的经验保留到下一级
        CurrentExp -= ExpToNextLevel;
        CurrentLevel++;

        // 计算下一级所需的经验（经验曲线）
        ExpToNextLevel = (ExpToNextLevel * expMultiplierPerLevel) + flatExpIncreasePerLevel;

        Debug.Log($"<color=#FFFF00>🌟 玩家升到了 {CurrentLevel} 级！</color>");

        // 🌟 核心劫持：向全宇宙广播玩家升级了！
        // UIManager 会监听到这个事件，然后：
        // 1. 暂停游戏 (Time.timeScale = 0)
        // 2. 弹出【晶片三选一】界面
        EventCenter.Broadcast(EventDefine.OnPlayerLevelUp, CurrentLevel);
    }
}
