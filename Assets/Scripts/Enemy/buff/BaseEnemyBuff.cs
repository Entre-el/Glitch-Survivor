// 🌟 纯 C# 类，极其轻量！不产生任何 GC 和组件开销
public abstract class BaseEnemyBuff
{
    public EnemyBuffSO buffData; // 引用的数据
    public float duration = 2f;       // 持续时间
    public float timeRemaining;  // 剩余时间
    protected EnemyCore target;  // 宿主
    public int stackCount = 0;

    // 构造函数：初始化 Buff
    public BaseEnemyBuff(EnemyBuffSO data, EnemyCore target,float duration = 2f)
    {
        this.buffData = data;
        this.duration = duration;
        this.timeRemaining = duration;
        this.target = target;
    }

    // 1. 刚挂上时的瞬间效果
    public virtual void OnApply() 
    {
        
    }
    public virtual void OnTick(float deltaTime) 
    {
        timeRemaining -= deltaTime;
    }
    public virtual void OnAddStack(int addedStacks = 1,float duration = 2f)
    {
        stackCount += addedStacks;
        timeRemaining = duration; // 刷新持续时间
    }

    // 3. 结束时的清理工作（比如恢复移速）
    public virtual void OnRemove() { }
}