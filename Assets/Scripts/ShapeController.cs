using UnityEngine;
using System.Collections.Generic;

public class ShapeController : MonoBehaviour
{
    [Header("Shape Data")]
    public ShapeData shapeData;
    
    [Header("Visual Settings")]
    public GameObject tilePrefab;
    public float tileSize = 0.9f;
    
    [Header("Drag Settings")]
    public float dragScale = 1.2f;
    public float returnSpeed = 15f;
    
    public System.Action OnShapePlaced;
    public bool IsPlaced { get { return isPlaced; } }
    
    private List<GameObject> shapeTiles = new List<GameObject>();
    private Vector3 originalPosition;
    private Vector3 dragOffset;
    private bool isDragging = false;
    private bool isPlaced = false;
    private Camera mainCamera;
    private Vector3 initialScale;
    
    void Start()
    {
        mainCamera = Camera.main;
        initialScale = transform.localScale;
        
        if (shapeData != null)
        {
            CreateVisualShape();
        }
        
        originalPosition = transform.position;
    }
    
    void CreateVisualShape()
    {
        Vector2Int[] tilePositions = shapeData.GetTilePositions();
        
        foreach (Vector2Int pos in tilePositions)
        {
            Vector3 tilePosition = new Vector3(pos.x * GridManager.Instance.cellSize, pos.y * GridManager.Instance.cellSize, 0);
            GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
            
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = shapeData.shapeColor;
            }
            
            tile.transform.localScale = Vector3.one * tileSize;
            shapeTiles.Add(tile);
        }
        
        // توسيط القطعة
        CenterShape();
    }
    
    void CenterShape()
    {
        Vector2Int[] positions = shapeData.GetTilePositions();
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        
        foreach (Vector2Int pos in positions)
        {
            min.x = Mathf.Min(min.x, pos.x);
            min.y = Mathf.Min(min.y, pos.y);
            max.x = Mathf.Max(max.x, pos.x);
            max.y = Mathf.Max(max.y, pos.y);
        }
        
        Vector3 centerOffset = new Vector3(
            (min.x + max.x) / 2f * GridManager.Instance.cellSize,
            (min.y + max.y) / 2f * GridManager.Instance.cellSize,
            0
        );
        
        foreach (GameObject tile in shapeTiles)
        {
            tile.transform.localPosition -= centerOffset;
        }
    }
    
    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            transform.position = worldPos + dragOffset;
        }
        else if (!isPlaced && Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            // إرجاع القطعة لمكانها الأصلي
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * returnSpeed);
        }
    }
    
    void OnMouseDown()
    {
        if (isPlaced) return;
        
        isDragging = true;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        dragOffset = transform.position - worldPos;
        
        // تكبير القطعة أثناء السحب
        transform.localScale = initialScale * dragScale;
        
        // رفع القطعة فوق الطبقات الأخرى
        foreach (GameObject tile in shapeTiles)
        {
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 100;
            }
        }
    }
    
    void OnMouseUp()
    {
        if (!isDragging || isPlaced) return;
        
        isDragging = false;
        transform.localScale = initialScale;
        
        // إعادة ترتيب الطبقات
        foreach (GameObject tile in shapeTiles)
        {
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 0;
            }
        }
        
        TryPlaceShape();
    }
    
    void TryPlaceShape()
    {
        GridManager grid = GridManager.Instance;
        Vector2Int[] tilePositions = shapeData.GetTilePositions();
        
        // حساب أقرب موقع في الشبكة
        Vector3 shapeCenter = transform.position;
        Vector2Int gridBasePos = grid.WorldToGridPosition(shapeCenter);
        
        // تعديل الموضع بناءً على مركز القطعة
        Vector2Int[] relativePositions = GetRelativePositions(tilePositions);
        
        // البحث عن أفضل موقع للإفلات
        Vector2Int bestPosition = FindBestPlacementPosition(grid, relativePositions, gridBasePos);
        
        if (grid.CanPlaceShape(relativePositions, bestPosition))
        {
            // وضع القطعة بنجاح
            PlaceShapeAtPosition(grid, relativePositions, bestPosition);
        }
        else
        {
            // العودة للموقع الأصلي
            StartCoroutine(ReturnToOriginal());
        }
    }
    
    Vector2Int[] GetRelativePositions(Vector2Int[] absolutePositions)
    {
        Vector2Int[] relative = new Vector2Int[absolutePositions.Length];
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        
        foreach (Vector2Int pos in absolutePositions)
        {
            min.x = Mathf.Min(min.x, pos.x);
            min.y = Mathf.Min(min.y, pos.y);
        }
        
        for (int i = 0; i < absolutePositions.Length; i++)
        {
            relative[i] = absolutePositions[i] - min;
        }
        
        return relative;
    }
    
    Vector2Int FindBestPlacementPosition(GridManager grid, Vector2Int[] relativePositions, Vector2Int basePos)
    {
        // البحث في منطقة صغيرة حول الموضع المحدد
        int searchRange = 2;
        Vector2Int bestPos = basePos;
        float bestDistance = float.MaxValue;
        
        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int y = -searchRange; y <= searchRange; y++)
            {
                Vector2Int testPos = basePos + new Vector2Int(x, y);
                
                if (grid.CanPlaceShape(relativePositions, testPos))
                {
                    Vector3 worldPos = grid.GridToWorldPosition(testPos);
                    float distance = Vector3.Distance(transform.position, worldPos);
                    
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestPos = testPos;
                    }
                }
            }
        }
        
        return bestPos;
    }
    
    void PlaceShapeAtPosition(GridManager grid, Vector2Int[] relativePositions, Vector2Int gridPosition)
    {
        Vector3 worldPos = grid.GridToWorldPosition(gridPosition);
        transform.position = worldPos;
        
        // تحديث الشبكة
        grid.PlaceShape(relativePositions, gridPosition, shapeData.shapeColor);
        
        isPlaced = true;
        
        // تعطيل السحب بعد الوضع
        enabled = false;
        
        // تأثير بصري عند الوضع
        StartCoroutine(PlaceEffect());
        
        // استدعاء الحدث
        OnShapePlaced?.Invoke();
        
        Debug.Log($"Shape '{shapeData.shapeName}' placed successfully at {gridPosition}");
    }
    
    System.Collections.IEnumerator ReturnToOriginal()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.3f;
            t = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            yield return null;
        }
        
        transform.position = originalPosition;
    }
    
    System.Collections.IEnumerator PlaceEffect()
    {
        // تأثير نبضة عند الوضع
        Vector3 targetScale = initialScale;
        transform.localScale = targetScale * 1.2f;
        
        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / 0.2f;
            transform.localScale = Vector3.Lerp(targetScale * 1.2f, targetScale, t);
            yield return null;
        }
        
        transform.localScale = targetScale;
    }
    
    public void Initialize(ShapeData data)
    {
        shapeData = data;
        if (shapeTiles.Count > 0)
        {
            foreach (GameObject tile in shapeTiles)
            {
                Destroy(tile);
            }
            shapeTiles.Clear();
        }
        CreateVisualShape();
    }
}