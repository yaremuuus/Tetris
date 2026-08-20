using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game UI")]
    public GameObject gameUI; 

    [Header("Game UI Buttons")]
    public Button settingsButton;
    public Button gamePlayButton;
    public Button pauseButton;   
    public Button stopButton;    

    [Header("References")]
    public SettingsManager settingsManager;
    public FigureSpawner figureSpawner;
    public MainMenuManager mainMenuManager; 
	public UIManager uiManager; 
    public AudioManager audioManager;

    private bool isGameStarted = false;
    private bool isPaused = false;

    void Start()
    {
        gameUI.SetActive(true);

        settingsButton.onClick.AddListener(ToggleSettings);
        gamePlayButton.onClick.AddListener(PlayGame);
        pauseButton.onClick.AddListener(PauseGame);
        stopButton.onClick.AddListener(StopGame);
    }

    public void OnGameStarted()
    {
        if (isGameStarted) return;
        isGameStarted = true;
        isPaused = false;
        Time.timeScale = 1f;

        if (uiManager != null) uiManager.ResetScore();

        if (audioManager != null)
        {
            audioManager.SetGameState(true);
            audioManager.PlayRandomMusic();
        }

        if (figureSpawner != null)
        {
            figureSpawner.GenerateGarbage();
            figureSpawner.SpawnNewFigure();
        }
    }

    public void PlayGame()
    {
        if (!isGameStarted) return;

        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    public void PauseGame()
    {
        if (!isGameStarted) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void StopGame()
    {
        if (!isGameStarted) return;

        if (audioManager != null)
        {
            audioManager.SetGameState(false);
            audioManager.StopMusic();
        }

        isGameStarted = false;
        isPaused = false;
        Time.timeScale = 1f;

        if (uiManager != null)
        {
            uiManager.CheckAndSaveRecord();
        }

        if (figureSpawner != null) figureSpawner.ResetSpawner();

        Figure activeFigure = FindAnyObjectByType<Figure>();
        if (activeFigure != null) Destroy(activeFigure.gameObject);

        GridManager grid = FindAnyObjectByType<GridManager>();
        if (grid != null) grid.ClearGrid();

        if (mainMenuManager != null)
        {
            mainMenuManager.ReturnToMainMenu();
        }
    }
    
    public void ToggleSettings()
    {
        if (settingsManager != null)
        {
            settingsManager.ToggleSettings();
        }
    }
	public bool IsGameActive()
    {
        return isGameStarted;
    }
	
}