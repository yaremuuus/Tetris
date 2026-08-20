using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text scoreText;   
    public TMP_Text recordText;  

    private int currentScore = 0;
    private int currentRecord = 0;

    private const string RECORD_KEY = "TetrisHighScore";

    void Start()
    {
        currentRecord = PlayerPrefs.GetInt(RECORD_KEY, 0);
        UpdateScoreUI();
        UpdateRecordUI();
    }

    public void AddPlacementScore(int blockCount)
    {
        currentScore += blockCount;
        UpdateScoreUI();
    }

    public void AddLineScore(int linesCleared)
    {
        int points = 0;
        switch (linesCleared)
        {
            case 1: points = 100; break;
            case 2: points = 300; break;
            case 3: points = 500; break;
            case 4: points = 800; break;
        }

        currentScore += points;
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void UpdateRecordUI()
    {
        if (recordText != null)
        {
            recordText.text = currentRecord.ToString("D1"); 
        }
    }

    public void CheckAndSaveRecord()
    {
        if (currentScore > currentRecord)
        {
            currentRecord = currentScore;
            PlayerPrefs.SetInt(RECORD_KEY, currentRecord);
            PlayerPrefs.Save(); 
            UpdateRecordUI(); 
        }

        currentScore = 0;
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }
}