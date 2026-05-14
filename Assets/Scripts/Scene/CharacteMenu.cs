using UnityEngine;

public class CharacterMenu : MonoBehaviour
{
    [Header("Scene Configuration")]
    public SceneSO nextScene; // 在 Inspector 中配置你要跳转的下一个场景

    void Awake()
    {
        EventCenter.AddListener(EventDefine.OnRequestSceneChange, ExecuteSceneTransition);
    }

    [System.Obsolete]
    void Start()
    {
        UIManager.Instance.ShowPanel<CharacterSelectionPanel>();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM("Menu");
        }
    }

    private void ExecuteSceneTransition()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.TransitionToScene(nextScene);
        }
        else
        {
            //Debug.LogError("致命异常：找不到全局的 SceneController，无法过图！");
        }
    }

    void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.OnRequestSceneChange, ExecuteSceneTransition);
    }
}
