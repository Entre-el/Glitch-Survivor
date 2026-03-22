using UnityEngine;
[CreateAssetMenu(fileName = "NewLevelData", menuName = "GameConfig/Scene Data")]
public class SceneSO : ScriptableObject
{
    [Header("场景基础信息")]
    [Tooltip("必须和 Build Settings 里的场景名字一模一样")]
    public string sceneName; 

    [Header("加载界面表现")]
    public string loadingMainText; // 比如："阴暗的地牢"
    [TextArea]
    public string[] loadingLogTexts; // 比如：["正在分配史莱姆内存...", "正在生成毒液陷阱..."]

    [Header("本关底层依赖")]
    public PoolItem[] requiredItems; // 把上一课的对象池配置也整合进来！
    [Header("Audio")]
    public AudioClip sceneBGM;
    public float cutInDuration = 2f;

}
