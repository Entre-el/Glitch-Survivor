using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        Win,
        LevelingUp
    }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentGameState;
    public GameState previousGameState;
    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public GameObject textObject;
    public float textFrontSize;
    public Camera referenceCamera;
    [Header("Screens")]
    public GameObject pauseScreens;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    public GameObject winScreen;
    [Header("Current Stats Display")]
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;
    [Header("Results Display")]
    public Image chosenCharacterIcon;
    public Text  chosenCharacterName;
    public Text  levelReachedDisplay;
    public Text  TimeSurvivedDisplay;
    public List<Image> chosenWeaponIcons = new(6);
    public List<Image> chosenPassiveIcons = new(6);
    [Header("Win Display")]
    public Image winCharacterIcon;
    public Text  winCharacterName;
    public Text  winLevelReachedDisplay;
    public Text  winTimeSurvivedDisplay;
    public List<Image> winChosenWeaponIcons = new(6);
    public List<Image> winChosenPassiveIcons = new(6);
    [Header("Stopwatch")]
    public float timeLimit;
    float stopwatchTime;
    public Text  stopwatchDisplay;
    [Header("Music")]
    public AudioClip winBGM;
    public AudioClip gameOverBGM;
    public bool isGameOver = false;
    public bool isLevelingUp = false;
    public bool isWin = false;
    public float cutInDuration = 2f;
    public GameObject playerObject;
    void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            EventCenter.AddListener(EventDefine.OnPlayerDied, OnPlayerDied);
        }
        else
        {
            Debug.LogWarning($"Multiple Instances of GameManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void OnPlayerDied()
    {
        SaveBattleData();
        EventCenter.Broadcast(EventDefine.OnGameOver);
    }
    private void SaveBattleData()
    {
        
    }

    public void ChangeState(GameState newState)
    {
        currentGameState = newState;
    }
    public void PauseGame()
    {
        if (currentGameState != GameState.Paused)
        {
            previousGameState = currentGameState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            AudioManager.Instance.bgmSource.pitch = 0.5f;
            pauseScreens.SetActive(true);
            Debug.Log($"Game Paused");
        }
    }
    public void ResumeGame()
    {
        if (currentGameState == GameState.Paused)
        {
            ChangeState(previousGameState);
            Time.timeScale = 1f;
            AudioManager.Instance.bgmSource.pitch = 1f;
            Debug.Log($"Game Resumed");
        }
    }
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void OnGameOver()
    {
        TimeSurvivedDisplay.text = stopwatchDisplay.text;
        AudioManager.Instance.CrossfadeBGM("GameOver",cutInDuration);
        ChangeState(GameState.GameOver);
    }
    public void OnWinGame()
    {
        TimeSurvivedDisplay.text = stopwatchDisplay.text;
        AudioManager.Instance.CrossfadeBGM("GameWin",cutInDuration);
        ChangeState(GameState.Win);
    }
    public void OnGamePause()
    {
        ChangeState(GameState.Paused);
    }
    public void OnGameResume()
    {
        ChangeState(GameState.Playing);
    }
    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;
        UpdateStopwatchDisplay();
        if (stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }
    void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60f);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60f);
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelingUp);
        playerObject.SendMessage("RemoveAndApplyUpgradeOptions");
    }
    public void EndLevelUp()
    {
        isLevelingUp = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Playing);
    }
    public static void GenerateDamageText(int damage, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!Instance.referenceCamera) Instance.referenceCamera = Camera.main;
        
        Instance.StartCoroutine(Instance.GenerateDamageTextCoroutine(damage, target, duration, speed));
    }

IEnumerator GenerateDamageTextCoroutine(int damage, Transform target, float duration, float speed)
    {
        if (!damageTextCanvas || !textObject)
        {
            Debug.LogError($"DamageTextCanvas or textObject is not assigned!");
            yield break;
        }
        TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
        if(textMesh is null)
        {
            Debug.LogError($"DamageText prefab 缺少 TextMeshPro 组件！");
            PoolItem item = textObject.GetComponent<PoolItem>();
            if(item is not null)
            {
                item.ReturnToPool();
            }
            else
            {
                Destroy(textObject);
            }
            yield break;
        }

        textMesh.SetText("{0}", damage);
        
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1f);

        Vector3 startPosition = Vector3.zero;
        if (target != null)
        {
            startPosition = target.position + new Vector3(0, 0.5f, 0); 
        }

        textObject.transform.position = startPosition;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration; 
            
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1f - progress);
            
            textObject.transform.position = startPosition + new Vector3(0, speed * t, 0);

            yield return null; 
        }

        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0f);
        
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