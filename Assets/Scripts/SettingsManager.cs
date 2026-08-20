using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References - Main UI")]
    public GameObject settingsPanel;    
    public Button settingsButton;       
    public Button applyButton;          

    [Header("Settings UI Elements")]
    public Slider speedSlider;        
    public TMP_Text speedText;        
    public Slider levelSlider;        
    public TMP_Text levelText;        
    public Toggle extraFiguresToggle; 
	public Toggle musicToggle; 

    [Header("Game References")]
    public GameObject gameVisuals;   
    public GameObject gameUI;        
    public FigureSpawner figureSpawner; 
    public MainMenuManager mainMenuManager; 
    public GameController gameController; 
	public AudioManager audioManager;
	
    private bool areSettingsOpen = false;
    private bool isGameActiveOnOpen = false;

    void Start()
    {
        settingsPanel.SetActive(false);
        musicToggle.isOn = true;
        speedSlider.value = 5f;
        speedText.text = (Mathf.RoundToInt(speedSlider.value * 5f)).ToString();
        levelSlider.value = 0f;
        levelText.text = "0";
        extraFiguresToggle.isOn = false;

        settingsButton.onClick.AddListener(ToggleSettings);
        applyButton.onClick.AddListener(CloseSettings);
    }

    public void ToggleSettings()
    {
        areSettingsOpen = !areSettingsOpen;

        if (areSettingsOpen)
        {
            OpenSettings();
        }
        else
        {
            CloseSettings();
        }
    }

    void OpenSettings()
    {
        if (gameController != null)
        {
            isGameActiveOnOpen = gameController.IsGameActive();
        }
        else
        {
            isGameActiveOnOpen = false;
        }

        settingsPanel.SetActive(true);
		musicToggle.isOn = audioManager.isMusicEnabled;
        
        speedSlider.value = figureSpawner.currentFallSpeed / 10f;
        speedText.text = (Mathf.RoundToInt(speedSlider.value * 10f)).ToString();
        
        levelSlider.value = figureSpawner.currentLevel;
        levelText.text = figureSpawner.currentLevel.ToString();

        extraFiguresToggle.isOn = figureSpawner.useExtraFigures; 

        if (gameVisuals != null && gameVisuals.activeSelf) gameVisuals.SetActive(false);
        if (gameUI != null && gameUI.activeSelf) gameUI.SetActive(false);

        Time.timeScale = 0f; 
    }

    void CloseSettings()
    {
        if (figureSpawner == null || speedSlider == null || speedText == null)
        {
            settingsPanel.SetActive(false);
            if (gameVisuals != null) gameVisuals.SetActive(true);
            if (gameUI != null) gameUI.SetActive(true);
            
            if (!isGameActiveOnOpen && mainMenuManager != null && mainMenuManager.mainMenuPanel != null)
            {
                mainMenuManager.mainMenuPanel.SetActive(true);
            }
            
            Time.timeScale = 1f;
            areSettingsOpen = false;
            return;
        }

        float speedValue = speedSlider.value;
        float actualSpeed = speedValue * 10f; 
        figureSpawner.currentFallSpeed = actualSpeed;
        speedText.text = (Mathf.RoundToInt(speedValue * 10f)).ToString();

        int levelValue = Mathf.RoundToInt(levelSlider.value);
        figureSpawner.currentLevel = levelValue;
        levelText.text = levelValue.ToString();

        figureSpawner.useExtraFigures = extraFiguresToggle.isOn;

        bool musicState = musicToggle.isOn;
        if (audioManager != null)
        {
            audioManager.SetMusicEnabled(musicState);
        }

        settingsPanel.SetActive(false);
        
        if (gameVisuals != null) gameVisuals.SetActive(true);
        if (gameUI != null) gameUI.SetActive(true);
        
        if (!isGameActiveOnOpen && mainMenuManager != null && mainMenuManager.mainMenuPanel != null)
        {
            mainMenuManager.mainMenuPanel.SetActive(true);
        }
        
        Time.timeScale = 1f;
        areSettingsOpen = false;
    }
}