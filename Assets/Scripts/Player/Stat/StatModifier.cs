public class StatModifier
{
    public readonly float Value;
    public readonly StatModType Type;
    public readonly int Order;
    public readonly object Source; // 关键：这是谁提供的？（比如是哪张贴纸提供的）

    // 构造函数
    public StatModifier(float value, StatModType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    // 简化版构造函数
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
}