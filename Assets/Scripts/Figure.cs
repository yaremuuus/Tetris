using UnityEngine;
using System.Collections.Generic;

public class Figure : MonoBehaviour
{
    [Header("Movement Settings")]
    public float fallSpeed = 2f; 
    public float fastFallMultiplier = 10f; 
    public float blockSize = 30f;      

    public System.Action<int> OnPlaced; 

    [Header("Components")]
    public GridManager gridManager;
    public FigureSpawner spawner;

    private List<Transform> blocks = new List<Transform>();
    private float holdDelay = 0.2f;      
    private float holdInterval = 0.05f;  
    private float leftHoldTimer = 0f;
    private float rightHoldTimer = 0f;
    private bool isLeftHeld = false;
    private bool isRightHeld = false;
    private Vector2Int gridPos;       
    private Vector2Int[] blockOffsets; 
    private float fallTimer = 0f;      
    private bool isPlaced = false;

    void Start()
    {
        foreach (Transform child in transform) blocks.Add(child);

        List<Vector2Int> offsets = new List<Vector2Int>();
        foreach (var block in blocks)
        {
            Vector3 localPos = transform.InverseTransformPoint(block.position);
            offsets.Add(new Vector2Int(
                Mathf.RoundToInt(localPos.x / blockSize), 
                Mathf.RoundToInt(localPos.y / blockSize)
            ));
        }
        blockOffsets = offsets.ToArray();

        if (gridManager == null) gridManager = FindAnyObjectByType<GridManager>();
        if (spawner == null) spawner = FindAnyObjectByType<FigureSpawner>(); 

        Vector2Int startGrid = gridManager.WorldToGrid(transform.position);
        gridPos = startGrid;
        UpdateVisualPosition();
    }

    void Update()
    {
        if (isPlaced || gridManager.IsProcessing()) return;

        HandleInput();
        HandleFall();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && CanMove(-1, 0)) 
        {
            gridPos.x = Mathf.Max(0, gridPos.x - 1); 
            UpdateVisualPosition();
            leftHoldTimer = 0f; 
            isLeftHeld = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && CanMove(1, 0))
        {
            gridPos.x = Mathf.Min(gridManager.gridWidth - 1, gridPos.x + 1); 
            UpdateVisualPosition();
            rightHoldTimer = 0f;
            isRightHeld = true;
        }

        if (Input.GetKey(KeyCode.LeftArrow) && isLeftHeld)
        {
            leftHoldTimer += Time.deltaTime;
            if (leftHoldTimer >= holdDelay)
            {
                if (CanMove(-1, 0))
                {
                    gridPos.x = Mathf.Max(0, gridPos.x - 1);
                    UpdateVisualPosition();
                }
                leftHoldTimer = holdDelay - holdInterval; 
            }
        }
        else if (!Input.GetKey(KeyCode.LeftArrow))
        {
            isLeftHeld = false;
        }

        if (Input.GetKey(KeyCode.RightArrow) && isRightHeld)
        {
            rightHoldTimer += Time.deltaTime;
            if (rightHoldTimer >= holdDelay)
            {
                if (CanMove(1, 0))
                {
                    gridPos.x = Mathf.Min(gridManager.gridWidth - 1, gridPos.x + 1);
                    UpdateVisualPosition();
                }
                rightHoldTimer = holdDelay - holdInterval;
            }
        }
        else if (!Input.GetKey(KeyCode.RightArrow))
        {
            isRightHeld = false;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            RotateFigure();
        }
    }

    void HandleFall()
    {
        float currentFallSpeed = (spawner != null) ? spawner.currentFallSpeed : fallSpeed;

        if (currentFallSpeed < 0.8f) currentFallSpeed = 0.8f;

        if (Input.GetKey(KeyCode.DownArrow)) 
            currentFallSpeed *= fastFallMultiplier;

        fallTimer += Time.deltaTime;
        while (fallTimer >= (1f / currentFallSpeed))
        {
            fallTimer -= (1f / currentFallSpeed); 

            if (CanMove(0, -1))
            {
                gridPos.y -= 1;
                UpdateVisualPosition();
            }
            else
            {
                PlaceFigure();
                return;
            }
        }
    }

    void UpdateVisualPosition()
    {
        Vector3 newPos = gridManager.GridToWorld(gridPos);
        transform.position = newPos;
    }

    public bool CanMove(int offsetX, int offsetY)
    {
        foreach (var offset in blockOffsets)
        {
            int targetX = gridPos.x + offset.x + offsetX;
            int targetY = gridPos.y + offset.y + offsetY;
            
            if (gridManager.IsCellOccupied(targetX, targetY)) return false;
        }
        return true;
    }

    void PlaceFigure()
    {
        isPlaced = true;

        int blockCount = blocks.Count;

        foreach (Transform block in blocks)
        {
            Vector2Int gridCoord = gridManager.WorldToGrid(block.position);
            block.SetParent(gridManager.transform);
            gridManager.AddBlockToGrid(gridCoord.x, gridCoord.y, block.gameObject);
        }

        if (OnPlaced != null) OnPlaced(blockCount);
        Destroy(gameObject);
    }

    void RotateFigure()
    {
        List<Vector2Int> newOffsets = new List<Vector2Int>();
        foreach (var offset in blockOffsets)
        {
            Vector2Int newOffset = new Vector2Int(offset.y, -offset.x); 
            
            int targetX = gridPos.x + newOffset.x;
            int targetY = gridPos.y + newOffset.y;
            
            if (gridManager.IsCellOccupied(targetX, targetY)) return; 
            newOffsets.Add(newOffset);
        }

        blockOffsets = newOffsets.ToArray();
        transform.Rotate(0, 0, -90); 
    }
}