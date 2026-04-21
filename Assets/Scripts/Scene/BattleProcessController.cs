using UnityEngine;

public class BattleProcessController : MonoBehaviour
{
    
    private void Start()
    {
        UIManger.Instance.ShowPanel<HpBarPanel>();
        UIManger.Instance.ShowPanel<ExpBarPanel>();
        EventCenter.AddListener(EventDefine.OnPlayerLevelUp, ShowLevelUpPanel);
    }
    private void ShowLevelUpPanel()
    {
        UIManger.Instance.ShowPanel<LevelUpPanel>();
    }
}
