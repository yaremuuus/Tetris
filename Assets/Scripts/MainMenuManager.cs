using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References - Main Menu")]
    public GameObject mainMenuPanel; 
    public Button playButtonMenu;    

    [Header("Game References")]
    public GameObject gameVisuals;   
    public GameController gameController; 

    private bool isGameStarted = false;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        
        playButtonMenu.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        if (isGameStarted) return;
        isGameStarted = true;

        mainMenuPanel.SetActive(false);

        if (gameController != null)
        {
            gameController.OnGameStarted();
        }
	
    }

    public void ReturnToMainMenu()
    {
        isGameStarted = false;
        mainMenuPanel.SetActive(true);
    }
}