using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public Transform shapesContainer;
    
    [Header("Shape Prefabs")]
    public GameObject shapeControllerPrefab;
    public GameObject tilePrefab;
    public GameObject gridCellPrefab;
    
    [Header("Game Settings")]
    public int shapesPerRound = 3;
    public Vector3[] shapeSpawnPositions;
    
    private ShapeController[] currentShapes;
    
    void Start()
    {
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
        }
        
        InitializeGame();
    }
    
    void InitializeGame()
    {
        SpawnNewShapes();
    }
    
    void SpawnNewShapes()
    {
        currentShapes = new ShapeController[shapesPerRound];
        
        for (int i = 0; i < shapesPerRound; i++)
        {
            ShapeData randomShape = GetRandomShape();
            Vector3 spawnPos = GetShapeSpawnPosition(i);
            
            GameObject shapeObj = Instantiate(shapeControllerPrefab, spawnPos, Quaternion.identity, shapesContainer);
            ShapeController controller = shapeObj.GetComponent<ShapeController>();
            
            if (controller != null)
            {
                controller.Initialize(randomShape);
                currentShapes[i] = controller;
            }
        }
    }
    
    ShapeData GetRandomShape()
    {
        int randomType = Random.Range(0, 5);
        
        switch (randomType)
        {
            case 0:
                return ShapeData.CreateShape_Square();
            case 1:
                return ShapeData.CreateShape_Line();
            case 2:
                return ShapeData.CreateShape_L();
            case 3:
                return ShapeData.CreateShape_T();
            case 4:
                return ShapeData.CreateShape_Single();
            default:
                return ShapeData.CreateShape_Single();
        }
    }
    
    Vector3 GetShapeSpawnPosition(int index)
    {
        float spacing = 3f;
        float startX = -((shapesPerRound - 1) * spacing) / 2f;
        
        return new Vector3(startX + index * spacing, -6f, 0);
    }
    
    public void CheckForNewShapes()
    {
        bool allPlaced = true;
        
        foreach (ShapeController shape in currentShapes)
        {
            if (shape != null && shape.enabled)
            {
                allPlaced = false;
                break;
            }
        }
        
        if (allPlaced)
        {
            Invoke(nameof(SpawnNewShapes), 0.5f);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearGrid();
        }
    }
    
    public void RestartGame()
    {
        // حذف القطع الحالية
        if (currentShapes != null)
        {
            foreach (ShapeController shape in currentShapes)
            {
                if (shape != null)
                {
                    Destroy(shape.gameObject);
                }
            }
        }
        
        // تفريغ الشبكة
        if (gridManager != null)
        {
            gridManager.ClearGrid();
        }
        
        // إنشاء قطع جديدة
        SpawnNewShapes();
        
        Debug.Log("Game restarted!");
    }
    
    public void ClearGrid()
    {
        if (gridManager != null)
        {
            gridManager.ClearGrid();
        }
        
        Debug.Log("Grid cleared!");
    }
}