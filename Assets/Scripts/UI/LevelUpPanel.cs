using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : BasePanel
{
    public VerticalLayoutGroup upgradeOptionsLayoutGroup;
    public GameObject upgradeOptions;
    private ScriptableObject weaponOrItemData;

    public override void OnShow()
    {
        base.OnShow();
        GameObject newOption = ObjectPoolManager.Instance.Get(upgradeOptions);
        newOption.transform.SetParent(upgradeOptionsLayoutGroup.transform);
    }
}
