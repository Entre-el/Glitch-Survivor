using UnityEngine;

public class SpinachPassiveItem : PassiveItem
{
    protected override void ApplyEffect()
    {
        playerStats. CurrentMight *= (1 + passiveItemData.Multiplier/100);
    }
}
