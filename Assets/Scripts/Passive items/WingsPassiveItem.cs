using UnityEngine;

public class WingsPassiveItem : PassiveItem
{
    protected override void ApplyEffect()
    {
        playerStats.CurrentMoveSpeed *= 1 + passiveItemData.Multiplier/100f;
    }
}
