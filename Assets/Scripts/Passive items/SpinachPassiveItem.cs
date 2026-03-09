using UnityEngine;

public class SpinachPassiveItem : PassiveItem
{
    protected override void ApplyEffect()
    {
        playerStats.CurrentMight = playerStats.characterData.Might*(1 + passiveItemData.Multiplier/100);
    }
}
