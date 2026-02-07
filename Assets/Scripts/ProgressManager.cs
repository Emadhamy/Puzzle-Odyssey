using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }
    
    private const string UNLOCKED_LEVELS_KEY = "UnlockedLevels";
    private const string LEVEL_STARS_KEY = "LevelStars_";
    private const string TOTAL_SCORE_KEY = "TotalScore";
    private const string TOTAL_JEWELS_KEY = "TotalJewels";
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeProgress()
    {
        // إذا كان أول تشغيل، افتح المرحلة الأولى فقط
        if (!PlayerPrefs.HasKey(UNLOCKED_LEVELS_KEY))
        {
            PlayerPrefs.SetInt(UNLOCKED_LEVELS_KEY, 1);
            PlayerPrefs.Save();
        }
    }
    
    // التحقق مما إذا كانت المرحلة مفتوحة
    public bool IsLevelUnlocked(int levelNumber)
    {
        int unlockedLevels = PlayerPrefs.GetInt(UNLOCKED_LEVELS_KEY, 1);
        return levelNumber <= unlockedLevels;
    }
    
    // فتح المرحلة التالية
    public void UnlockNextLevel(int currentLevel)
    {
        int unlockedLevels = PlayerPrefs.GetInt(UNLOCKED_LEVELS_KEY, 1);
        if (currentLevel >= unlockedLevels)
        {
            PlayerPrefs.SetInt(UNLOCKED_LEVELS_KEY, currentLevel + 1);
            PlayerPrefs.Save();
            Debug.Log($"Level {currentLevel + 1} unlocked!");
        }
    }
    
    // حفظ نجوم المرحلة
    public void SaveLevelStars(int levelNumber, int stars)
    {
        string key = LEVEL_STARS_KEY + levelNumber;
        int currentStars = PlayerPrefs.GetInt(key, 0);
        
        // حفظ أعلى عدد من النجوم
        if (stars > currentStars)
        {
            PlayerPrefs.SetInt(key, stars);
            PlayerPrefs.Save();
        }
    }
    
    // الحصول على نجوم المرحلة
    public int GetLevelStars(int levelNumber)
    {
        string key = LEVEL_STARS_KEY + levelNumber;
        return PlayerPrefs.GetInt(key, 0);
    }
    
    // الحصول على عدد المراحل المفتوحة
    public int GetUnlockedLevelsCount()
    {
        return PlayerPrefs.GetInt(UNLOCKED_LEVELS_KEY, 1);
    }
    
    // حفظ النقاط
    public void AddScore(int score)
    {
        int totalScore = PlayerPrefs.GetInt(TOTAL_SCORE_KEY, 0);
        totalScore += score;
        PlayerPrefs.SetInt(TOTAL_SCORE_KEY, totalScore);
        PlayerPrefs.Save();
    }
    
    public int GetTotalScore()
    {
        return PlayerPrefs.GetInt(TOTAL_SCORE_KEY, 0);
    }
    
    // حفظ الجواهر
    public void AddJewels(int jewels)
    {
        int totalJewels = PlayerPrefs.GetInt(TOTAL_JEWELS_KEY, 0);
        totalJewels += jewels;
        PlayerPrefs.SetInt(TOTAL_JEWELS_KEY, totalJewels);
        PlayerPrefs.Save();
    }
    
    public int GetTotalJewels()
    {
        return PlayerPrefs.GetInt(TOTAL_JEWELS_KEY, 0);
    }
    
    // إعادة تعيين جميع التقدم
    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(UNLOCKED_LEVELS_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("All progress reset!");
    }
    
    // فتح جميع المراحل (للاختبار)
    public void UnlockAllLevels(int totalLevels)
    {
        PlayerPrefs.SetInt(UNLOCKED_LEVELS_KEY, totalLevels);
        PlayerPrefs.Save();
    }
}