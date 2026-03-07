using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats playerStats;
    public PassiveItemScriptableObject passiveItemData;
    protected virtual void ApplyEffect()
    {
        // 由具体被动子类覆写，实现属性加成/效果逻辑
    }
    void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        ApplyEffect();
    }
}
