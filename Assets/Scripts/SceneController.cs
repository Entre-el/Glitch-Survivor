using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // 必须引入 TMP 来做极其极客的文字渲染
using UnityEngine.UI;
using System.Linq;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    [Header("硬核加载界面")]
    public GameObject loadingScreen; // 挂载一个黑底面板，平时隐藏
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText; // 用来极速打印底层日志的 TMP 文字
    void Awake()
    {
        if (instance is null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SceneChange(SceneSO sceneData)
    {
        Time.timeScale = 1f;
        StartCoroutine(HardcoreLoadSequence(sceneData));
    }

    private IEnumerator HardcoreLoadSequence(SceneSO sceneData)
    {
        loadingScreen.SetActive(true);
        loadingSlider.value = 0f;
        // 准备一个安全的取词器（防呆设计：防止策划在 SO 里少填了文字导致数组越界报错）
        string GetLog(int index, string fallback) 
        {
            if (sceneData.loadingLogTexts != null && index < sceneData.loadingLogTexts.Length)
                return "> " + sceneData.loadingLogTexts[index];
            return "> " + fallback;
        }

        loadingText.SetText($"> 建立连接: {sceneData.loadingMainText}");
        yield return new WaitForSeconds(0.2f); // 视觉缓冲

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneData.sceneName);
        asyncLoad.allowSceneActivation = false; 
        loadingSlider.value = 0.1f;
        loadingText.SetText(GetLog(0, "正在读取底层场景资产..."));
        while (asyncLoad.progress < 0.9f) 
        { 
            yield return null; 
        }
        loadingSlider.value = 0.2f;
        loadingText.SetText(GetLog(1, "正在预分配对象池内存区块..."));
        yield return null; // 极其关键：必须等一帧，让上面的 UI 文字真正在屏幕上渲染出来！
        loadingSlider.value = 0.3f;
        // 开始执行产生巨大运算量的代码
        if (sceneData.requiredItems != null && sceneData.requiredItems.Length > 0)
        {
            ObjectPoolManager.Instance.InitializePools(sceneData.requiredItems.Select(p => p.gameObject).ToArray());
        }
        loadingSlider.value = 0.9f;
        loadingText.SetText(GetLog(2, "执行强制 GC 内存碎片回收..."));
        yield return null; 
        System.GC.Collect();
        loadingSlider.value = 1f;
        loadingText.SetText(GetLog(4, "系统重组完毕。进入实战。"));
        yield return new WaitForSeconds(0.15f); // 故意停留极其短暂的瞬间，营造一种“系统跑得太快了”的错觉
        AudioManager.instance.CrossfadeBGM(sceneData.sceneBGM,sceneData.cutInDuration);
        asyncLoad.allowSceneActivation = true;
    }
}