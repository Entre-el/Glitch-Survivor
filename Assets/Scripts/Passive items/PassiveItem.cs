using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats playerStats;
    public PassiveItemScriptableObject passiveItemData;
    protected virtual void ApplyEffect()
    {
        // This method should be overridden in derived classes to apply the specific effect of the passive item
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindAnyObjectByType<PlayerStats>();
        ApplyEffect();
    }
}
