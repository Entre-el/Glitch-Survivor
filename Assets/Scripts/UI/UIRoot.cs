using UnityEngine;

// 挂载在每个场景自己的 Canvas 上
public class UIRoot : MonoBehaviour
{
    private BasePanel[] localPanels;

    private void Start()
    {
        // 1. 场景一诞生，话事人立刻把当前场景里所有的 UI 奴隶揪出来
        localPanels = GetComponentsInChildren<BasePanel>(true);
        foreach (var panel in localPanels)
        {
            UIManger.Instance.RegisterPanel(panel);
        }
    }

    private void OnDestroy()
    {
        
        if (UIManger.Instance != null) // 防御性编程，防止 UIManager 已经先死了
        {
            foreach (var panel in localPanels)
            {
                UIManger.Instance.UnregisterPanel(panel);
            }
        }
    }
}