using System;
using UnityEngine;

public enum GameState
{
    Playing,
    Paused,
    GameOver,
    Win,
    LevelingUp,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("游戏状态")]
    public GameState currentGameState = GameState.Playing;
    public GameState previousGameState; // 用于暂停恢复时回退状态

    [Header("全局配置")]
    public float cutInDuration = 2f;

    // 不再用 GameObject，直接拿 PlayerCore 的引用，彻底消灭 SendMessage
    public PlayerCore playerCore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 🌟 注册事件
            EventCenter.AddListener(EventDefine.OnPlayerDied, OnPlayerDied);
        }
        else
        {
            //Debug.LogWarning("检测到多个 GameManager 实例，正在销毁多余实例。");
            Destroy(gameObject);
        }
    }

    // 🌟 养成好习惯：有 AddListener 就必须有 RemoveListener！
    void OnDestroy()
    {
        if (Instance == this)
        {
            EventCenter.RemoveListener(EventDefine.OnPlayerDied, OnPlayerDied);
        }
    }

    private void Update()
    {
        // 🌟 将 ESC 暂停游戏的逻辑收束在这里
        // 注意：如果你在 UIManager 里也写了 ESC，你需要协调一下。
        // 最好的做法是：按下 ESC 时，如果当前是 Playing，就呼出暂停菜单。
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        previousGameState = currentGameState;
        currentGameState = newState;
    }

    private void OnPlayerDied()
    {
        if (currentGameState == GameState.GameOver)
            return; // 防止重复触发

        SaveBattleData();
        OnGameOver();
    }

    private void SaveBattleData()
    {
        // 你的保存逻辑
    }

    public void PauseGame()
    {
        if (currentGameState == GameState.Playing)
        {
            ChangeState(GameState.Paused);

            // UI 和时间流逝交给 UIManager 处理
            UIManager.Instance.ShowPanel<PausePanel>();

            // GameManager 只负责非 UI 的全局表现（比如音效变调）
            if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
                AudioManager.Instance.bgmSource.pitch = 0.5f;

            //Debug.Log("Game Paused");
        }
    }

    public void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            ChangeState(previousGameState);

            // 通知 UI 关闭暂停面板
            UIManager.Instance.HidePanel<PausePanel>();

            if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
                AudioManager.Instance.bgmSource.pitch = 1f;

            //Debug.Log("Game Resumed");
        }
    }

    public void OnGameOver()
    {
        ChangeState(GameState.GameOver);

        if (AudioManager.Instance != null)
            AudioManager.Instance.CrossfadeBGM("GameOver", cutInDuration);

        // 呼出结算面板
        UIManager.Instance.ShowPanel<ResultsPanel>();
        EventCenter.Broadcast(EventDefine.OnGameOver);
    }

    public void OnWinGame()
    {
        ChangeState(GameState.Win);

        if (AudioManager.Instance != null)
            AudioManager.Instance.CrossfadeBGM("GameWin", cutInDuration);

        UIManager.Instance.ShowPanel<WinPanel>();
    }

    [Obsolete]
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelingUp);
        UIManager.Instance.ShowPanel<LevelUpPanel>();

        // 🌟 彻底抛弃 SendMessage！直接调用对应脚本的方法，或者走 EventCenter
        // if (playerCore != null && playerCore.UpgradeManager != null)
        // {
        //     playerCore.UpgradeManager.RemoveAndApplyUpgradeOptions();
        // }
    }

    public void EndLevelUp()
    {
        UIManager.Instance.HidePanel<LevelUpPanel>();
        ChangeState(GameState.Playing);
    }

    public void QuitGame()
    {
        EventCenter.Broadcast(EventDefine.OnGameQuit);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
