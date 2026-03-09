using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 单例：用于全局访问当前局的状态与 UI
    public static GameManager instance;
    public enum GameState
    {
        Playing,
        Paused,
        GameOver,
        LevelingUp
    }
    public GameState currentGameState;
    public GameState previousGameState;
    [Header("Screens")] 
    public GameObject pauseScreens; 
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    [Header("Current Stats Display")]
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;
    [Header("Results Display")]
    public Image chosenCharacterIcon;
    public Text chosenCharacterName;
    public Text levelReachedDisplay;
    public Text TimeSurvivedDisplay;
    public List<Image> chosenWeaponIcons = new(6);
    public List<Image> chosenPassiveIcons = new(6);
    [Header("Stopwatch")]
    // 存活时间上限（秒）
    public float timeLimit;
    float stopwatchTime;
    public Text stopwatchDisplay;


    public bool isGameOver = false;
    public bool isLevelingUp = false;
    public GameObject playerObject;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
        DisableScreens();
    }
    void Update()
    {
        switch(currentGameState)
        {
            case GameState.Playing:
                    CheckForPauseAndResume();
                    UpdateStopwatch();
                break;
            case GameState.Paused:
                    CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if(!isGameOver){
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game Over");
                    DisplayResults();
                }
                break;
            case GameState.LevelingUp:
                if(!isLevelingUp)
                {
                    isLevelingUp = true;
                    Time.timeScale = 0f;
                    levelUpScreen.SetActive(true);
                    Debug.Log("Level Up!");
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
        if(currentGameState != GameState.Paused)
        {
            previousGameState = currentGameState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreens.SetActive(true);
            Debug.Log("Game Paused");
        }
    }
    public void ResumeGame()
    {
        if(currentGameState == GameState.Paused)
        {
            ChangeState(previousGameState);
            Time.timeScale = 1f;
            DisableScreens();
            Debug.Log("Game Resumed");
        }
    }
    void CheckForPauseAndResume()
    {
         if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentGameState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    void DisableScreens()
    {
        pauseScreens.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }
    public void GameOver()
    {
        // 结算时展示本局存活时间
        TimeSurvivedDisplay.text = stopwatchDisplay.text;
        ChangeState(GameState.GameOver);
    }
    void DisplayResults()
    {
        resultsScreen.SetActive(true); 
    }
    public void AssignChosenCharacterUI(CharacterScriptableObject character)
    {
        chosenCharacterIcon.sprite = character.Icon;
        chosenCharacterName.text = character.Name;
    }
    public  void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text =  levelReachedData.ToString();
    }
    public void AssignChosenWeaponUI(List<Image> chosenWeaponIconsData,List<Image> chosenPassiveIconsData)
    {
        if(chosenWeaponIconsData.Count != chosenWeaponIcons.Count || chosenPassiveIconsData.Count != chosenPassiveIcons.Count)
        {
            Debug.LogWarning("Mismatch in the number of weapon/passive icons provided.");
            return;
        }
        for(int i = 0; i < chosenWeaponIcons.Count; i++)
        {
            chosenWeaponIcons[i].enabled = false;
            if(chosenWeaponIconsData[i].enabled)
            {
                chosenWeaponIcons[i].enabled = true;
                chosenWeaponIcons[i].sprite = chosenWeaponIconsData[i].sprite;
            }
        }
        for(int j = 0; j < chosenPassiveIcons.Count; j++)
        {
            chosenPassiveIcons[j].enabled = false;
            if(chosenPassiveIconsData[j].enabled)
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
        if(stopwatchTime >= timeLimit)
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
        // 进入升级：暂停时间、打开升级界面，并让玩家刷新可选升级按钮
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
}
