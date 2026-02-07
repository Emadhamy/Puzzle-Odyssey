using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    [Header("References")]
    public GridManager gridManager;
    public UIManager uiManager;
    public ShapeSpawner shapeSpawner;
    
    [Header("Level Data")]
    public LevelData[] allLevels;
    public LevelData currentLevelData;
    
    [Header("Game State")]
    public int currentLevel = 1;
    public int currentScore = 0;
    public int collectedJewels = 0;
    public int movesCount = 0;
    public bool isGameActive = false;
    public bool isPaused = false;
    
    [Header("Scoring")]
    public int basePoints = 10;
    public int lineClearBonus = 100;
    public int jewelBonus = 50;
    
    private List<Vector2Int> activeBombs = new List<Vector2Int>();
    
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
    
    void Start()
    {
        // تحميل المرحلة الحالية
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        LoadLevel(currentLevel);
    }
    
    public void LoadLevel(int levelNumber)
    {
        currentLevel = levelNumber;
        
        // البحث عن بيانات المرحلة
        if (allLevels != null && levelNumber <= allLevels.Length)
        {
            currentLevelData = allLevels[levelNumber - 1];
        }
        else
        {
            // إنشاء مرحلة افتراضية إذا لم توجد بيانات
            currentLevelData = CreateDefaultLevelData(levelNumber);
        }
        
        InitializeLevel();
    }
    
    void InitializeLevel()
    {
        // إعادة تعيين الحالة
        currentScore = 0;
        collectedJewels = 0;
        movesCount = 0;
        isGameActive = true;
        isPaused = false;
        activeBombs.Clear();
        
        // تهيئة الشبكة
        if (gridManager != null)
        {
            gridManager.InitializeGrid(currentLevelData);
        }
        
        // توليد القطع
        if (shapeSpawner != null)
        {
            shapeSpawner.SpawnShapes(currentLevelData);
        }
        
        // تحديث واجهة المستخدم
        UpdateUI();
        
        Debug.Log($"Level {currentLevel} started! Type: {currentLevelData.levelType}");
    }
    
    public void OnShapePlaced(int tilesCount)
    {
        if (!isGameActive) return;
        
        movesCount++;
        
        // إضافة النقاط
        int points = tilesCount * basePoints;
        currentScore += points;
        
        // تقليل عداد القنابل
        DecrementBombs();
        
        // التحقق من انفجار القنابل
        if (CheckBombExplosions())
        {
            GameOver();
            return;
        }
        
        // التحقق من مسح الصفوف
        CheckLineClears();
        
        // التحقق من شروط الفوز
        CheckWinConditions();
        
        UpdateUI();
    }
    
    public void OnJewelCollected()
    {
        collectedJewels++;
        currentScore += jewelBonus;
        
        // تأثير صوتي/بصري
        Debug.Log("Jewel collected!");
        
        CheckWinConditions();
        UpdateUI();
    }
    
    void DecrementBombs()
    {
        foreach (Vector2Int bombPos in activeBombs.ToArray())
        {
            GridCell cell = gridManager.GetCell(bombPos.x, bombPos.y);
            if (cell != null)
            {
                cell.DecrementBomb();
            }
        }
    }
    
    bool CheckBombExplosions()
    {
        foreach (Vector2Int bombPos in activeBombs)
        {
            GridCell cell = gridManager.GetCell(bombPos.x, bombPos.y);
            if (cell != null && cell.IsBombExploded())
            {
                Debug.Log($"Bomb exploded at ({bombPos.x}, {bombPos.y})!");
                return true;
            }
        }
        return false;
    }
    
    void CheckLineClears()
    {
        List<int> clearedRows = gridManager.CheckAndClearLines();
        
        if (clearedRows.Count > 0)
        {
            // إضافة نقاط إضافية
            int bonus = clearedRows.Count * lineClearBonus;
            currentScore += bonus;
            
            // في مراحل الجليد، قد تختفي المكعبات الثلجية
            if (currentLevelData.levelType == LevelType.IceBlocks)
            {
                ClearIceInRows(clearedRows);
            }
            
            Debug.Log($"Cleared {clearedRows.Count} lines! Bonus: {bonus}");
        }
    }
    
    void ClearIceInRows(List<int> rows)
    {
        foreach (int row in rows)
        {
            for (int x = 0; x < gridManager.gridWidth; x++)
            {
                GridCell cell = gridManager.GetCell(x, row);
                if (cell != null && cell.specialType == SpecialCellType.Ice)
                {
                    cell.ClearIce();
                }
            }
        }
    }
    
    void CheckWinConditions()
    {
        bool won = false;
        
        switch (currentLevelData.levelType)
        {
            case LevelType.ScoreGoal:
                won = currentScore >= currentLevelData.goals.targetScore;
                break;
                
            case LevelType.IceBlocks:
                won = gridManager.GetRemainingIceCount() == 0;
                break;
                
            case LevelType.Jewels:
                won = collectedJewels >= currentLevelData.goals.targetJewels;
                break;
                
            case LevelType.TimeBomb:
                won = gridManager.GetRemainingBombCount() == 0 && 
                      activeBombs.Count == 0;
                break;
                
            case LevelType.BossLevel:
                // المراحل الزعيم: تحقق من النقاط وإزالة جميع العناصر الخاصة
                bool scoreMet = currentScore >= currentLevelData.goals.targetScore;
                bool specialCleared = gridManager.GetTotalSpecialCellsCount() == 0;
                won = scoreMet && specialCleared;
                break;
        }
        
        if (won)
        {
            LevelComplete();
        }
    }
    
    void LevelComplete()
    {
        isGameActive = false;
        
        // حساب النجوم
        int stars = CalculateStars();
        
        // حفظ التقدم
        ProgressManager.Instance.SaveLevelStars(currentLevel, stars);
        ProgressManager.Instance.UnlockNextLevel(currentLevel);
        ProgressManager.Instance.AddScore(currentScore);
        ProgressManager.Instance.AddJewels(collectedJewels);
        
        // عرض شاشة النجاح
        if (uiManager != null)
        {
            uiManager.ShowLevelComplete(currentScore, stars);
        }
        
        Debug.Log($"Level {currentLevel} completed! Stars: {stars}");
    }
    
    void GameOver()
    {
        isGameActive = false;
        
        // عرض شاشة الخسارة
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
        
        Debug.Log("Game Over!");
    }
    
    int CalculateStars()
    {
        // حساب النجوم بناءً على الأداء
        int stars = 1; // نجمة واحدة على الأقل للفوز
        
        switch (currentLevelData.levelType)
        {
            case LevelType.ScoreGoal:
                if (currentScore >= currentLevelData.goals.targetScore * 1.5f) stars = 3;
                else if (currentScore >= currentLevelData.goals.targetScore * 1.2f) stars = 2;
                break;
                
            case LevelType.Jewels:
                if (movesCount <= currentLevelData.goals.targetJewels * 2) stars = 3;
                else if (movesCount <= currentLevelData.goals.targetJewels * 3) stars = 2;
                break;
                
            default:
                // لبقية الأنواع
                if (movesCount <= 20) stars = 3;
                else if (movesCount <= 35) stars = 2;
                break;
        }
        
        return stars;
    }
    
    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        
        if (uiManager != null)
        {
            uiManager.ShowPauseMenu(isPaused);
        }
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1;
        LoadLevel(currentLevel);
    }
    
    public void NextLevel()
    {
        if (currentLevel < 50)
        {
            LoadLevel(currentLevel + 1);
        }
    }
    
    public void GoToLevelSelect()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect");
    }
    
    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(currentScore);
            uiManager.UpdateJewels(collectedJewels, currentLevelData.goals.targetJewels);
            uiManager.UpdateMoves(movesCount);
        }
    }
    
    LevelData CreateDefaultLevelData(int level)
    {
        LevelData data = ScriptableObject.CreateInstance<LevelData>();
        data.levelNumber = level;
        data.levelName = $"Level {level}";
        
        // تحديد نوع المرحلة
        if (level <= 10)
        {
            data.levelType = LevelType.ScoreGoal;
            data.goals.targetScore = 500 + (level * 100);
        }
        else if (level <= 20)
        {
            data.levelType = LevelType.IceBlocks;
            data.goals.targetScore = 1000;
            // إضافة مكعبات ثلجية عشوائية
            List<Vector2Int> icePositions = new List<Vector2Int>();
            for (int i = 0; i < 5 + level - 10; i++)
            {
                icePositions.Add(new Vector2Int(Random.Range(0, 8), Random.Range(0, 8)));
            }
            data.iceBlockPositions = icePositions.ToArray();
        }
        else if (level <= 30)
        {
            data.levelType = LevelType.Jewels;
            data.goals.targetJewels = 3 + (level - 20);
            // إضافة جواهر
            List<Vector2Int> jewelPositions = new List<Vector2Int>();
            for (int i = 0; i < data.goals.targetJewels + 2; i++)
            {
                jewelPositions.Add(new Vector2Int(Random.Range(0, 8), Random.Range(0, 8)));
            }
            data.jewelPositions = jewelPositions.ToArray();
        }
        else if (level <= 40)
        {
            data.levelType = LevelType.TimeBomb;
            data.goals.bombMoves = 15 - (level - 30);
            // إضافة قنابل
            List<Vector2Int> bombPositions = new List<Vector2Int>();
            for (int i = 0; i < 3 + (level - 30) / 2; i++)
            {
                bombPositions.Add(new Vector2Int(Random.Range(0, 8), Random.Range(0, 8)));
            }
            data.bombPositions = bombPositions.ToArray();
        }
        else
        {
            data.levelType = LevelType.BossLevel;
            data.goals.targetScore = 3000;
            data.goals.preFilledPercentage = 40;
        }
        
        return data;
    }
}