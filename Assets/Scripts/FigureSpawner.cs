using UnityEngine;
using System.Collections.Generic;

public class FigureSpawner : MonoBehaviour
{
    [Header("Figure Prefabs")]
    public GameObject[] standardFigurePrefabs; 
    public GameObject[] extraFigurePrefabs;    

	public GameObject[] blockPrefabs;

    public GridManager gridManager;
    public Transform spawnPoint; 
    public Transform previewContainer; 
    public UIManager uiManager; 
    public GameController gameController; 

    [Header("Settings")]
    public float currentFallSpeed = 2f; 
    public int currentLevel = 0; 
    public bool useExtraFigures = false; 

    private int nextFigureIndex = -1;
    private bool hasGeneratedGarbage = false;

    void Start()
    {
        nextFigureIndex = Random.Range(0, standardFigurePrefabs.Length);
    }

        public void SpawnNewFigure()
    {
        
        gridManager.ProcessLines();
        
        if (gridManager.CheckGameOver(spawnPoint.position))
        {
            if (gameController != null)
            {
                gameController.StopGame();
            }
            return; 
        }

       
        List<GameObject> availableFigures = new List<GameObject>();
        availableFigures.AddRange(standardFigurePrefabs);
        if (useExtraFigures && extraFigurePrefabs != null && extraFigurePrefabs.Length > 0)
        {
            availableFigures.AddRange(extraFigurePrefabs);
        }
        
        if (availableFigures.Count == 0) return;

       
        int currentIndex = nextFigureIndex;
        nextFigureIndex = Random.Range(0, availableFigures.Count);
        
        GameObject newFigure = Instantiate(availableFigures[currentIndex], spawnPoint.position, Quaternion.identity, gridManager.transform.parent);

        Figure figureScript = newFigure.GetComponent<Figure>();
        
        if (figureScript != null)
        {
            figureScript.gridManager = gridManager;
            figureScript.fallSpeed = currentFallSpeed;

            figureScript.OnPlaced += (blockCount) => 
            {
                if (uiManager != null) uiManager.AddPlacementScore(blockCount);
                SpawnNewFigure(); 
            };
        }

        UpdatePreview();
    }

    void UpdatePreview()
    {
        List<GameObject> availableFigures = new List<GameObject>();
        availableFigures.AddRange(standardFigurePrefabs);
        if (useExtraFigures && extraFigurePrefabs != null && extraFigurePrefabs.Length > 0)
        {
            availableFigures.AddRange(extraFigurePrefabs);
        }

        foreach (Transform child in previewContainer)
        {
            Destroy(child.gameObject);
        }

        if (availableFigures.Count == 0) return;

        GameObject previewFigure = Instantiate(availableFigures[nextFigureIndex], previewContainer.position, Quaternion.identity);
        previewFigure.transform.SetParent(previewContainer);
        Figure figScript = previewFigure.GetComponent<Figure>();
        if (figScript != null) Destroy(figScript);
    }

    public void ResetSpawner()
    {
        CancelInvoke();
        nextFigureIndex = Random.Range(0, standardFigurePrefabs.Length);
        foreach (Transform child in previewContainer)
        {
            Destroy(child.gameObject);
        }
        hasGeneratedGarbage = false; 
    }

    public void GenerateGarbage()
    {
        if (currentLevel == 0) return; 
        if (hasGeneratedGarbage) return; 

        if (gridManager == null) return;
        if (blockPrefabs == null || blockPrefabs.Length == 0)
        {
            return;
        }

        hasGeneratedGarbage = true;
        int rows = currentLevel;
        int width = gridManager.gridWidth;

        for (int y = 0; y < rows; y++)
        {
            List<int> freeCells = new List<int>();
            for (int x = 0; x < width; x++) freeCells.Add(x);

            int blocksCount = Random.Range(3, 7);
            
            for (int i = 0; i < blocksCount; i++)
            {
                int randIndex = Random.Range(0, freeCells.Count);
                int x = freeCells[randIndex];
                freeCells.RemoveAt(randIndex);

                int randomPrefabIndex = Random.Range(0, blockPrefabs.Length);
                GameObject trashBlock = Instantiate(blockPrefabs[randomPrefabIndex], 
                                                    gridManager.GridToWorld(new Vector2Int(x, y)), 
                                                    Quaternion.identity);

                trashBlock.transform.SetParent(gridManager.transform);
                gridManager.AddBlockToGrid(x, y, trashBlock);
            }
        }
    }
}