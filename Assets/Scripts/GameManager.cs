using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Paused,
        GameOver
    }
    public GameState currentGameState;
    public GameState previousGameState;
    [Header("UI")] 
    public GameObject PauseScreens; 
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;
    void Awake()
    {
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
            PauseScreens.SetActive(true); // Show the pause menu UI
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
        PauseScreens.SetActive(false);
    }
}