using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float cellSize = 1f;
    public GameObject cellPrefab;
    
    [Header("Visual Settings")]
    public Color defaultCellColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color highlightColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    
    private GridCell[,] gridCells;
    private Vector3 gridOrigin;
    
    public static GridManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void InitializeGrid(LevelData levelData)
    {
        // مسح الشبكة القديمة إذا وجدت
        ClearExistingGrid();
        
        gridWidth = levelData.gridWidth;
        gridHeight = levelData.gridHeight;
        
        CreateGrid();
        SetupSpecialCells(levelData);
        
        // إذا كانت مرحلة زعيم، املأ الشبكة جزئياً
        if (levelData.levelType == LevelType.BossLevel)
        {
            PreFillGrid(levelData);
        }
    }
    
    void ClearExistingGrid()
    {
        if (gridCells != null)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
    
    void CreateGrid()
    {
        gridCells = new GridCell[gridWidth, gridHeight];
        
        // حساب نقطة البداية لوضع الشبكة في المنتصف
        float totalWidth = gridWidth * cellSize;
        float totalHeight = gridHeight * cellSize;
        gridOrigin = new Vector3(-totalWidth / 2f + cellSize / 2f, -totalHeight / 2f + cellSize / 2f, 0);
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 cellPosition = new Vector3(
                    gridOrigin.x + x * cellSize,
                    gridOrigin.y + y * cellSize,
                    0
                );
                
                GameObject cellObj = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{y}";
                
                GridCell cell = cellObj.GetComponent<GridCell>();
                if (cell != null)
                {
                    cell.Initialize(x, y);
                    gridCells[x, y] = cell;
                }
            }
        }
    }
    
    void SetupSpecialCells(LevelData levelData)
    {
        // إعداد المكعبات الثلجية
        foreach (Vector2Int pos in levelData.iceBlockPositions)
        {
            if (IsValidPosition(pos))
            {
                gridCells[pos.x, pos.y].SetIceBlock();
            }
        }
        
        // إعداد الجواهر
        foreach (Vector2Int pos in levelData.jewelPositions)
        {
            if (IsValidPosition(pos))
            {
                gridCells[pos.x, pos.y].SetJewel();
            }
        }
        
        // إعداد القنابل
        foreach (Vector2Int pos in levelData.bombPositions)
        {
            if (IsValidPosition(pos))
            {
                gridCells[pos.x, pos.y].SetBomb(levelData.goals.bombMoves);
            }
        }
    }
    
    void PreFillGrid(LevelData levelData)
    {
        int cellsToFill = (gridWidth * gridHeight * levelData.goals.preFilledPercentage) / 100;
        int filled = 0;
        
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!gridCells[x, y].isOccupied)
                {
                    availablePositions.Add(new Vector2Int(x, y));
                }
            }
        }
        
        // خلط المواقع
        for (int i = 0; i < availablePositions.Count; i++)
        {
            int randomIndex = Random.Range(i, availablePositions.Count);
            Vector2Int temp = availablePositions[i];
            availablePositions[i] = availablePositions[randomIndex];
            availablePositions[randomIndex] = temp;
        }
        
        // ملء الخلايا
        foreach (Vector2Int pos in availablePositions)
        {
            if (filled >= cellsToFill) break;
            
            Color randomColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
            gridCells[pos.x, pos.y].SetOccupied(randomColor);
            filled++;
        }
    }
    
    public bool IsValidPosition(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridWidth &&
               gridPos.y >= 0 && gridPos.y < gridHeight;
    }
    
    public bool IsCellOccupied(Vector2Int gridPos)
    {
        if (!IsValidPosition(gridPos))
            return true;
        return gridCells[gridPos.x, gridPos.y].isOccupied;
    }
    
    public GridCell GetCell(int x, int y)
    {
        if (IsValidPosition(new Vector2Int(x, y)))
        {
            return gridCells[x, y];
        }
        return null;
    }
    
    public bool CanPlaceShape(Vector2Int[] shapeTiles, Vector2Int basePosition)
    {
        foreach (Vector2Int tile in shapeTiles)
        {
            Vector2Int targetPos = basePosition + tile;
            if (!IsValidPosition(targetPos))
            {
                return false;
            }
            
            GridCell cell = gridCells[targetPos.x, targetPos.y];
            if (cell != null && !cell.CanPlaceBlock())
            {
                return false;
            }
        }
        return true;
    }
    
    public void PlaceShape(Vector2Int[] shapeTiles, Vector2Int basePosition, Color shapeColor)
    {
        foreach (Vector2Int tile in shapeTiles)
        {
            Vector2Int targetPos = basePosition + tile;
            if (IsValidPosition(targetPos))
            {
                gridCells[targetPos.x, targetPos.y].SetOccupied(shapeColor);
            }
        }
        
        // إشعار LevelManager بوضع القطعة
        LevelManager.Instance.OnShapePlaced(shapeTiles.Length);
    }
    
    public List<int> CheckAndClearLines()
    {
        List<int> clearedRows = new List<int>();
        List<int> clearedCols = new List<int>();
        
        // التحقق من الصفوف
        for (int y = 0; y < gridHeight; y++)
        {
            bool rowFull = true;
            for (int x = 0; x < gridWidth; x++)
            {
                if (!gridCells[x, y].isOccupied)
                {
                    rowFull = false;
                    break;
                }
            }
            
            if (rowFull)
            {
                clearedRows.Add(y);
            }
        }
        
        // التحقق من الأعمدة
        for (int x = 0; x < gridWidth; x++)
        {
            bool colFull = true;
            for (int y = 0; y < gridHeight; y++)
            {
                if (!gridCells[x, y].isOccupied)
                {
                    colFull = false;
                    break;
                }
            }
            
            if (colFull)
            {
                clearedCols.Add(x);
            }
        }
        
        // مسح الصفوف
        foreach (int row in clearedRows)
        {
            ClearRow(row);
        }
        
        // مسح الأعمدة
        foreach (int col in clearedCols)
        {
            ClearColumn(col);
        }
        
        // جمع الجواهر الممسوحة
        CollectClearedJewels(clearedRows, clearedCols);
        
        return clearedRows;
    }
    
    void ClearRow(int row)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            GridCell cell = gridCells[x, row];
            
            // إذا كانت خلية جليدية، تختفي مع الصف
            if (cell.specialType == SpecialCellType.Ice)
            {
                cell.ClearIce();
            }
            else
            {
                cell.Clear();
            }
        }
    }
    
    void ClearColumn(int col)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            GridCell cell = gridCells[col, y];
            
            // إذا كانت خلية جليدية، تختفي مع العمود
            if (cell.specialType == SpecialCellType.Ice)
            {
                cell.ClearIce();
            }
            else
            {
                cell.Clear();
            }
        }
    }
    
    void CollectClearedJewels(List<int> rows, List<int> cols)
    {
        // جمع الجواهر في الصفوف الممسوحة
        foreach (int row in rows)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (gridCells[x, row].specialType == SpecialCellType.Jewel)
                {
                    LevelManager.Instance.OnJewelCollected();
                }
            }
        }
        
        // جمع الجواهر في الأعمدة الممسوحة
        foreach (int col in cols)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridCells[col, y].specialType == SpecialCellType.Jewel)
                {
                    LevelManager.Instance.OnJewelCollected();
                }
            }
        }
    }
    
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - gridOrigin;
        int x = Mathf.RoundToInt(localPos.x / cellSize);
        int y = Mathf.RoundToInt(localPos.y / cellSize);
        return new Vector2Int(x, y);
    }
    
    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(
            gridOrigin.x + gridPosition.x * cellSize,
            gridOrigin.y + gridPosition.y * cellSize,
            0
        );
    }
    
    public void ClearGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridCells[x, y] != null)
                {
                    gridCells[x, y].Clear();
                }
            }
        }
    }
    
    // إحصائيات
    public int GetRemainingIceCount()
    {
        int count = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridCells[x, y].specialType == SpecialCellType.Ice)
                {
                    count++;
                }
            }
        }
        return count;
    }
    
    public int GetRemainingJewelCount()
    {
        int count = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridCells[x, y].specialType == SpecialCellType.Jewel)
                {
                    count++;
                }
            }
        }
        return count;
    }
    
    public int GetRemainingBombCount()
    {
        int count = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridCells[x, y].specialType == SpecialCellType.Bomb)
                {
                    count++;
                }
            }
        }
        return count;
    }
    
    public int GetTotalSpecialCellsCount()
    {
        return GetRemainingIceCount() + GetRemainingJewelCount() + GetRemainingBombCount();
    }
}