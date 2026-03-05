using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance of the GameManager
    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }
    public GameState currentGameState;
    public GameState previousGameState;
    [Header("Screens")] 
    public GameObject pauseScreens; 
    public GameObject resultsScreen;
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
    public bool isGameOver = false;
    void Awake()
    {
        if(instance == null)
        {
            instance = this; // Set the singleton instance
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameManager detected. Destroying duplicate.");
            Destroy(gameObject); // Ensure only one instance exists
        }
        DisableScreens(); // Hide the pause menu UI at the start
    }
    void Update()
    {
        switch(currentGameState)
        {
            case GameState.Playing:
                    CheckForPauseAndResume();
                break;
            case GameState.Paused:
                    CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if(!isGameOver){
                    isGameOver = true;
                    Time.timeScale = 0f; // Pause the game
                    Debug.Log("Game Over");
                    DisplayResults();
                }
                break;
            default:
                Time.timeScale = 1f; // Default to normal time
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
            previousGameState = currentGameState; // Store the previous state before pausing
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Pause the game
            pauseScreens.SetActive(true); // Show the pause menu UI
            Debug.Log("Game Paused");
        }
    }
    public void ResumeGame()
    {
        if(currentGameState == GameState.Paused)
        {
            ChangeState(previousGameState); // Restore the previous state when resuming
            Time.timeScale = 1f; // Resume the game
            DisableScreens(); // Hide the pause menu UI
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
    }
    public void GameOver()
    {
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
    public void AssignTimeSurvivedUI(int timeSurvivedData)
    {
        TimeSurvivedDisplay.text = timeSurvivedData.ToString() + "seconds";
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
            chosenWeaponIcons[i].enabled = false; // Disable the icon if no weapon is assigned
            if(chosenWeaponIconsData[i].enabled) // Check if the weapon icon data is enabled before assigning
            {
                chosenWeaponIcons[i].enabled = true; // Ensure the icon is enabled
                chosenWeaponIcons[i].sprite = chosenWeaponIconsData[i].sprite;
            }
        }
        for(int j = 0; j < chosenPassiveIcons.Count; j++)
        {
            chosenPassiveIcons[j].enabled = false; // Disable the icon if no passive is assigned
            if(chosenPassiveIconsData[j].enabled)
            {
                chosenPassiveIcons[j].enabled = true; // Ensure the icon is enabled
                chosenPassiveIcons[j].sprite = chosenPassiveIconsData[j].sprite;
            }
        }
    }
}
