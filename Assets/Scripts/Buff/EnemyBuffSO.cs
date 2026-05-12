using UnityEngine;

public enum BuffType
{
    Poison,
    Slow,
    Stun,
    AttackBoost,
    Drunk,
    Marked,
    Vulnerable,
}

[CreateAssetMenu(menuName = "EnemyBuff", fileName = "SO/EnemyBuffSO")]
public class EnemyBuffSO : ScriptableObject
{
    public BuffType buffType;
    public string buffName;

    [TextArea]
    public string buffDescription;
    public Sprite buffIcon;
}
