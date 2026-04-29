using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // 唯一的过图入口
    public void TransitionToScene(SceneSO sceneData)
    {
        StartCoroutine(HardcoreLoadSequence(sceneData));
    }

    [System.Obsolete]
    private IEnumerator HardcoreLoadSequence(SceneSO sceneData)
    {
        // 1. 锁死时间，清理旧 UI（把 MenuPanel 关掉！）
        Time.timeScale = 1f;
        UIManager.Instance.PopTopPanel(); // 或者明确 HidePanel<MenuPanel>()

        // 2. 召唤 Loading 面板
        UIManager.Instance.ShowPanel<LoadingPanel>();
        // 必须要等一帧，让 UIManager 把面板真正拽出来！
        yield return null;

        // 强行拿到刚才弹出来的 Loading 面板
        // (需要在 UIManager 里加个 GetPanel<T> 方法，或者用更解耦的事件，这里为了直观直接赋值)
        LoadingPanel loadingUI = UIManager.Instance.GetPanel<LoadingPanel>();

        // 3. 阶段一：建立连接 (视觉欺骗)
        loadingUI.UpdateProgress(0.1f, $"建立连接: {sceneData.loadingMainText}");
        yield return new WaitForSeconds(0.1f);

        // 4. 阶段二：底层真实加载！
        loadingUI.UpdateProgress(0.2f, "正在读取底层场景资产...");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneName);
        asyncLoad.allowSceneActivation = false;

        // 🚨 极其核心的物理防线：必须死死卡在这里，直到引擎真的加载到 90%！
        while (asyncLoad.progress < 0.9f)
        {
            // Unity 的 progress 是 0 到 0.9，我们把它映射到 UI 的 0.2 到 0.8
            float uiProgress = Mathf.Lerp(0.2f, 0.8f, asyncLoad.progress / 0.9f);
            loadingUI.UpdateProgress(uiProgress, "正在读取底层场景资产...");
            yield return null;
        }

        // 5. 阶段三：引擎加载完毕，开始业务加载（对象池）
        loadingUI.UpdateProgress(0.85f, "正在预分配对象池内存区块...");
        yield return null; // 缓冲一帧让文字刷出来
        if (sceneData.requiredItems != null && sceneData.requiredItems.Count > 0)
        {
            ObjectPoolManager.Instance.InitializePools(
                sceneData.requiredItems.Select(p => p.gameObject).ToArray()
            );
        }

        // 6. 阶段四：物理内存重组
        loadingUI.UpdateProgress(0.95f, "执行强制 GC 内存碎片回收...");
        yield return null;
        System.GC.Collect();

        // 7. 阶段五：完美跨越生死
        loadingUI.UpdateProgress(1f, "系统重组完毕。进入实战。");
        yield return new WaitForSeconds(0.15f); // 最后的视觉停留

        // 切 BGM (假设 AudioManager 也是永生的)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.CrossfadeBGM(sceneData.sceneBGM, sceneData.cutInDuration);
        }

        // 放行！进入新场景！
        asyncLoad.allowSceneActivation = true;

        // 隐藏 Loading 面板
        UIManager.Instance.HidePanel<LoadingPanel>();
    }
}
