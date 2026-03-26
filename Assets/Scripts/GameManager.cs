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
    public static GameManager instance;
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
        if (instance is null)
        {
            instance = this;
            EventCenter.AddListener(EventDefine.OnGameStart, OnGameStart);
            EventCenter.AddListener(EventDefine.OnGameOver, OnGameOver);
            EventCenter.AddListener(EventDefine.OnGameWin, OnGameWin);
            EventCenter.AddListener(EventDefine.OnGamePause, OnGamePause);
            EventCenter.AddListener(EventDefine.OnGameResume, OnGameResume);
            EventCenter.AddListener(EventDefine.OnGameRestart, OnGameRestart);
            EventCenter.AddListener(EventDefine.OnGameQuit, OnGameQuit);
        }
        else
        {
            Debug.LogWarning($"Multiple instances of GameManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    void Update()
    {
        switch (currentGameState)
        {
            case GameState.Playing:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log($"Game Over");
                    DisplayResults();
                }
                break;
            case GameState.Win:
                if (!isWin)
                {
                    isWin = true;
                    Time.timeScale = 0f;
                    Debug.Log($"Win");
                    DisplayWin();
                }
                break;
            case GameState.LevelingUp:
                if (!isLevelingUp)
                {
                    isLevelingUp = true;
                    Time.timeScale = 0f;
                    levelUpScreen.SetActive(true);
                    Debug.Log($"Level Up!");
                }
                break;
            default:
                Time.timeScale = 1f;
                break;
        }
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
            AudioManager.instance.bgmSource.pitch = 0.5f;
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
            AudioManager.instance.bgmSource.pitch = 1f;
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
        AudioManager.instance.CrossfadeBGM("GameOver",cutInDuration);
        ChangeState(GameState.GameOver);
    }
    public void OnWinGame()
    {
        TimeSurvivedDisplay.text = stopwatchDisplay.text;
        AudioManager.instance.CrossfadeBGM("GameWin",cutInDuration);
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
    public void OnGameRestart()
    {
        resultsScreen.SetActive(true);
    }
    void DisplayWin()
    {   
        WinScreen.SetActive(true);
    }
    public void AssignChosenCharacterUI(CharacterScriptableObject character)
    {
        chosenCharacterIcon.sprite = character.Icon;
        chosenCharacterName.text = character.Name;
    }
    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }
    public void AssignChosenWeaponUI(List<Image> chosenWeaponIconsData, List<Image> chosenPassiveIconsData)
    {
        if (chosenWeaponIconsData.Count != chosenWeaponIcons.Count || chosenPassiveIconsData.Count != chosenPassiveIcons.Count)
        {
            Debug.LogWarning($"Mismatch in the number of weapon/passive icons provided.");
            return;
        }
        for (int i = 0; i < chosenWeaponIcons.Count; i++)
        {
            chosenWeaponIcons[i].enabled = false;
            if (chosenWeaponIconsData[i].enabled)
            {
                chosenWeaponIcons[i].enabled = true;
                chosenWeaponIcons[i].sprite = chosenWeaponIconsData[i].sprite;
            }
        }
        for (int j = 0; j < chosenPassiveIcons.Count; j++)
        {
            chosenPassiveIcons[j].enabled = false;
            if (chosenPassiveIconsData[j].enabled)
            {
                chosenPassiveIcons[j].enabled = true;
                chosenPassiveIcons[j].sprite = chosenPassiveIconsData[j].sprite;
            }
        }
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
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;
        
        instance.StartCoroutine(instance.GenerateDamageTextCoroutine(damage, target, duration, speed));
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