using UnityEngine;

public enum LevelType
{
    ScoreGoal,      // المراحل 1-10: أهداف نقاط
    IceBlocks,      // المراحل 11-20: مكعبات ثلجية
    Jewels,         // المراحل 21-30: تحدي الجواهر
    TimeBomb,       // المراحل 31-40: قنبلة موقوتة
    BossLevel       // المراحل 41-50: مراحل الزعيم
}

public enum CellType
{
    Empty,          // خلية فارغة
    Normal,         // مكعب عادي
    Ice,            // مكعب ثلجي
    Jewel,          // جوهرة
    Bomb            // قنبلة
}

[System.Serializable]
public class LevelGoal
{
    public int targetScore;           // الهدف من النقاط
    public int targetJewels;          // عدد الجواهر المطلوب جمعها
    public int bombMoves;             // عدد الحركات قبل انفجار القنبلة
    public int preFilledPercentage;   // نسبة الشبكة المملوءة مسبقاً (للزعيم)
}

[System.Serializable]
public class CellData
{
    public int x;
    public int y;
    public CellType cellType;
    public Color blockColor;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Puzzle Odyssey/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber;
    public string levelName;
    public LevelType levelType;
    public bool isLocked = true;
    public int starsEarned = 0;
    
    [Header("Level Goals")]
    public LevelGoal goals;
    
    [Header("Grid Setup")]
    public int gridWidth = 8;
    public int gridHeight = 8;
    public CellData[] preFilledCells;  // الخلايا المملوءة مسبقاً
    
    [Header("Special Blocks")]
    public Vector2Int[] iceBlockPositions;    // مواقع المكعبات الثلجية
    public Vector2Int[] jewelPositions;       // مواقع الجواهر
    public Vector2Int[] bombPositions;        // مواقع القنابل
    
    [Header("Available Shapes")]
    public bool allowSquare = true;
    public bool allowLine = true;
    public bool allowL = true;
    public bool allowT = true;
    public bool allowSingle = true;
    
    [Header("Difficulty")]
    [Range(1, 10)]
    public int difficulty = 1;
    
    public bool HasSpecialBlocks()
    {
        return iceBlockPositions.Length > 0 || 
               jewelPositions.Length > 0 || 
               bombPositions.Length > 0;
    }
    
    public int GetTotalSpecialBlocks()
    {
        return iceBlockPositions.Length + jewelPositions.Length + bombPositions.Length;
    }
}