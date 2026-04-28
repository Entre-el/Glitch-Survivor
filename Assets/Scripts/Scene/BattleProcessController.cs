using UnityEngine;

public class BattleProcessController : MonoBehaviour
{
    private BasePanel[] panels;

    private void Start()
    {
        // 1. 场景一诞生，话事人立刻把当前场景里所有的 UI 奴隶揪出来
        panels = GetComponentsInChildren<BasePanel>(true);

        foreach (var panel in panels)
        {
            UIManager.Instance.RegisterAndHidePanel(panel);
        }
        //2. 话事人把这些 UI 奴隶统统展示出来
        UIManager.Instance.ShowPanel<HpBarPanel>(false);
        UIManager.Instance.ShowPanel<ExpBarPanel>(false);

        EventCenter.AddListener(EventDefine.OnPlayerLevelUp, ShowLevelUpPanel);
    }

    private void Update()
    {
        // 快捷键打开背包逻辑
        if (Input.GetKeyDown(KeyCode.B))
            ToggleStickerInventory();
    }

    public void ToggleStickerInventory()
    {
        UIManager.Instance.TogglePanel<StickerInventoryPanel>();
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null) // 防御性编程，防止 UIManager 已经先死了
        {
            foreach (var panel in panels)
            {
                UIManager.Instance.UnregisterPanel(panel);
            }
        }
        EventCenter.RemoveListener(EventDefine.OnPlayerLevelUp, ShowLevelUpPanel);
    }

    private void ShowLevelUpPanel()
    {
        UIManager.Instance.ShowPanel<LevelUpPanel>();
    }
}
