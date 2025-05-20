using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //Define the different states of game
    public enum GameState
    {
        Gameplay,
        Paused,
        Gameover,
        GameWin
    }

    //Store the current gamestate
    public GameState currentState;
    //Store the previous gamestate
    public GameState previousState;

    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject gameoverScreen;
    public GameObject gameWinScreen;

    [Header("Stopwatch")]
    public float timeLimit = 210f; 
    float stopwatchTime; 
    public Text stopwatchDisplay;

    void Awake()
    {
        DisableScreens();
    }

    void Update()
    {

        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.Gameover:
                break;

            case GameState.GameWin:
                break;
            
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }

        Victory();
    }

    //Define the method to change the state of the game
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {

        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            Debug.Log("Game is Paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            Debug.Log("Game is resumed");
        }
    }

    //Method to check if the game is paused already, determines if pauses or resumes
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
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
        pauseScreen.SetActive(false);
        gameoverScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.Gameover);
        Time.timeScale = 0f;
        DisableScreens();
        gameoverScreen.SetActive(true);
    }
    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopWatchDisplay();
    }

    void UpdateStopWatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }
    
    void Victory()
    {
        if (stopwatchTime >= timeLimit)
        {
            ChangeState(GameState.GameWin);
            Time.timeScale = 0f;
            DisableScreens();
            gameWinScreen.SetActive(true);
        }
    }

}


