using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats playerStats;
    public PassiveItemScriptableObject passiveItemData;

    protected virtual void ApplyEffect()
    {
    }

    void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        ApplyEffect();
    }
}
