using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    private AsyncOperation asyncLoad;
    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EventCenter.AddListener<SceneSO>(EventDefine.OnRequestSceneChange, OnReceiveSceneChangeRequest);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnReceiveSceneChangeRequest(SceneSO sceneData)
    {
        SceneChange(sceneData);
    }
    public void SceneChange(SceneSO sceneData)
    {
        Time.timeScale = 1f;
        StartCoroutine(HardcoreLoadSequence(sceneData));
    }

    private IEnumerator HardcoreLoadSequence(SceneSO sceneData)
    {
        UIManger.Instance.ShowPanel<LoadingPanel>();
        yield return null;
        EventCenter.Broadcast(EventDefine.OnLoadingStart, sceneData);

        asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneName);
        asyncLoad.allowSceneActivation = false; 

        if (sceneData.requiredItems != null && sceneData.requiredItems.Length > 0)
        {
            ObjectPoolManager.Instance.InitializePools(sceneData.requiredItems.Select(p => p.gameObject).ToArray());
        }
        EventCenter.Broadcast(EventDefine.OnPoolInit, sceneData);
        
        EventCenter.AddListener<SceneSO>(EventDefine.OnLoadingScreenFinished, OnSceneLoadingPerformanceFinished);
    }
    private void HandleGameOverSequence()
    {
        AudioManager.Instance.PlayBGM(SceneBGM.GameOver.ToString());
        // UIManger.Instance.ShowPanel<GameOverPanel>();
    }
    private void OnSceneLoadingPerformanceFinished(SceneSO sceneData)
    {  
        EventCenter.RemoveListener<SceneSO>(EventDefine.OnLoadingScreenFinished,OnSceneLoadingPerformanceFinished);
        asyncLoad.allowSceneActivation = true;
        AudioManager.Instance.CrossfadeBGM(sceneData.sceneBGM,sceneData.cutInDuration);
        UIManger.Instance.HidePanel<LoadingPanel>();
    }
}
public enum SceneBGM
{
    MainMenu,
    Gameplay,
    GameOver,
    GameWin,
}