using UnityEngine;
using System.Collections.Generic;

public class ShapeSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject shapeControllerPrefab;
    public Transform shapesContainer;
    
    [Header("Spawn Settings")]
    public float spawnSpacing = 3f;
    public float spawnY = -6f;
    
    private ShapeController[] currentShapes;
    private LevelData currentLevelData;
    
    public void SpawnShapes(LevelData levelData)
    {
        currentLevelData = levelData;
        
        // حذف القطع القديمة
        ClearExistingShapes();
        
        int shapesCount = 3; // عدد القطع في كل جولة
        currentShapes = new ShapeController[shapesCount];
        
        float startX = -((shapesCount - 1) * spawnSpacing) / 2f;
        
        for (int i = 0; i < shapesCount; i++)
        {
            Vector3 spawnPos = new Vector3(startX + i * spawnSpacing, spawnY, 0);
            
            GameObject shapeObj = Instantiate(shapeControllerPrefab, spawnPos, Quaternion.identity, shapesContainer);
            ShapeController controller = shapeObj.GetComponent<ShapeController>();
            
            if (controller != null)
            {
                ShapeData randomShape = GetRandomShapeForLevel(levelData);
                controller.Initialize(randomShape);
                controller.OnShapePlaced += OnShapePlaced;
                currentShapes[i] = controller;
            }
        }
    }
    
    void ClearExistingShapes()
    {
        if (currentShapes != null)
        {
            foreach (ShapeController shape in currentShapes)
            {
                if (shape != null)
                {
                    shape.OnShapePlaced -= OnShapePlaced;
                    Destroy(shape.gameObject);
                }
            }
        }
        
        // حذف أي قطع أخرى في الحاوية
        foreach (Transform child in shapesContainer)
        {
            Destroy(child.gameObject);
        }
    }
    
    ShapeData GetRandomShapeForLevel(LevelData levelData)
    {
        List<System.Func<ShapeData>> availableShapes = new List<System.Func<ShapeData>>();
        
        if (levelData.allowSquare) availableShapes.Add(ShapeData.CreateShape_Square);
        if (levelData.allowLine) availableShapes.Add(ShapeData.CreateShape_Line);
        if (levelData.allowL) availableShapes.Add(ShapeData.CreateShape_L);
        if (levelData.allowT) availableShapes.Add(ShapeData.CreateShape_T);
        if (levelData.allowSingle) availableShapes.Add(ShapeData.CreateShape_Single);
        
        if (availableShapes.Count == 0)
        {
            return ShapeData.CreateShape_Single();
        }
        
        int randomIndex = Random.Range(0, availableShapes.Count);
        return availableShapes[randomIndex]();
    }
    
    void OnShapePlaced()
    {
        // التحقق مما إذا كانت جميع القطع قد وُضعت
        bool allPlaced = true;
        int placedCount = 0;
        
        foreach (ShapeController shape in currentShapes)
        {
            if (shape != null && !shape.IsPlaced)
            {
                allPlaced = false;
            }
            else if (shape == null || shape.IsPlaced)
            {
                placedCount++;
            }
        }
        
        // إذا وُضعت جميع القطع، قم بتوليد قطع جديدة
        if (allPlaced)
        {
            Invoke(nameof(SpawnNewShapes), 0.5f);
        }
    }
    
    void SpawnNewShapes()
    {
        if (LevelManager.Instance.isGameActive)
        {
            SpawnShapes(currentLevelData);
        }
    }
}