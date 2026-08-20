using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;   
    public int gridHeight = 20;  
    public float blockSize = 30f; 
    public UIManager uiManager; 

    private GameObject[,] grid;
    public System.Action<int> OnLineCleared;

    private bool isProcessing = false; 

    void Start()
    {
        grid = new GameObject[gridWidth, gridHeight];
    }

    public bool IsProcessing()
    {
        return isProcessing;
    }

    public bool IsCellOccupied(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return true;
        return grid[x, y] != null;
    }

    public void AddBlockToGrid(int x, int y, GameObject blockObject)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            grid[x, y] = blockObject;
        }
    }

    public bool ProcessLines()
    {
        if (isProcessing) return false;
        isProcessing = true; 

        int linesCleared = 0;
        bool changed = false;

        for (int y = 0; y < gridHeight; y++)
        {
            if (IsLineFull(y))
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    if (grid[x, y] != null)
                    {
                        Destroy(grid[x, y]);
                        grid[x, y] = null;
                    }
                }
                linesCleared++;
                changed = true;

                for (int row = y + 1; row < gridHeight; row++)
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        if (grid[x, row] != null)
                        {
                            grid[x, row - 1] = grid[x, row];
                            grid[x, row] = null;
                            grid[x, row - 1].transform.position = GridToWorld(new Vector2Int(x, row - 1));
                        }
                    }
                }
                y--; 
            }
        }

        isProcessing = false; 

        if (changed)
        {
            if (uiManager != null) uiManager.AddLineScore(linesCleared);

            if (OnLineCleared != null) OnLineCleared(linesCleared);
        }
        return changed;
    }

    bool IsLineFull(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x, y] == null) return false;
        }
        return true;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - transform.position.x - blockSize / 2f) / blockSize);
        int y = Mathf.RoundToInt((worldPos.y - transform.position.y - blockSize / 2f) / blockSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = transform.position.x + gridPos.x * blockSize + blockSize / 2f;
        float y = transform.position.y + gridPos.y * blockSize + blockSize / 2f;
        return new Vector3(x, y, 0);
    }
    
    public void ClearGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y]);
                    grid[x, y] = null;
                }
            }
        }
    }

    public bool CheckGameOver(Vector3 spawnPosition)
    {
        Vector2Int spawnGrid = WorldToGrid(spawnPosition);
        
        if (IsCellOccupied(spawnGrid.x, spawnGrid.y - 1))
        {
            return true;
        }
        return false;
    }
}