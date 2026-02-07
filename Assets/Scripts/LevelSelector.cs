using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public static LevelSelector Instance { get; private set; }
    
    [Header("UI References")]
    public Transform buttonsContainer;
    public GameObject levelButtonPrefab;
    public TextMeshProUGUI pageNumberText;
    public Button nextPageButton;
    public Button previousPageButton;
    public Button backButton;
    
    [Header("Settings")]
    public int totalLevels = 50;
    public int levelsPerPage = 20;
    public int totalPages = 3;
    
    [Header("Info Panel")]
    public GameObject levelInfoPanel;
    public TextMeshProUGUI levelInfoText;
    public Button playButton;
    
    private int currentPage = 1;
    private int selectedLevel = 0;
    private LevelButton[] levelButtons;
    
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
        InitializeLevelButtons();
        UpdatePageDisplay();
        SetupNavigationButtons();
        
        // إخفاء لوحة المعلومات في البداية
        if (levelInfoPanel != null)
        {
            levelInfoPanel.SetActive(false);
        }
    }
    
    void InitializeLevelButtons()
    {
        levelButtons = new LevelButton[totalLevels];
        
        for (int i = 0; i < totalLevels; i++)
        {
            int levelNumber = i + 1;
            
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonsContainer);
            LevelButton levelButton = buttonObj.GetComponent<LevelButton>();
            
            if (levelButton != null)
            {
                bool unlocked = ProgressManager.Instance.IsLevelUnlocked(levelNumber);
                int stars = ProgressManager.Instance.GetLevelStars(levelNumber);
                
                levelButton.Setup(levelNumber, unlocked, stars);
                levelButtons[i] = levelButton;
            }
        }
    }
    
    void SetupNavigationButtons()
    {
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        backButton.onClick.AddListener(GoBack);
        
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlaySelectedLevel);
        }
        
        UpdateNavigationButtons();
    }
    
    void NextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
            UpdatePageDisplay();
        }
    }
    
    void PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            UpdatePageDisplay();
        }
    }
    
    void UpdatePageDisplay()
    {
        pageNumberText.text = $"Page {currentPage}/{totalPages}";
        
        // تحديد أي الأزرار تظهر
        int startLevel = (currentPage - 1) * levelsPerPage;
        int endLevel = Mathf.Min(startLevel + levelsPerPage, totalLevels);
        
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].gameObject.SetActive(i >= startLevel && i < endLevel);
            }
        }
        
        UpdateNavigationButtons();
    }
    
    void UpdateNavigationButtons()
    {
        previousPageButton.interactable = currentPage > 1;
        nextPageButton.interactable = currentPage < totalPages;
    }
    
    public void LoadLevel(int levelNumber)
    {
        selectedLevel = levelNumber;
        
        // عرض معلومات المرحلة
        ShowLevelInfo(levelNumber);
    }
    
    void ShowLevelInfo(int levelNumber)
    {
        if (levelInfoPanel != null)
        {
            levelInfoPanel.SetActive(true);
            
            // تحديد نوع المرحلة
            string levelTypeText = GetLevelTypeText(levelNumber);
            int stars = ProgressManager.Instance.GetLevelStars(levelNumber);
            
            levelInfoText.text = $"Level {levelNumber}\n" +
                                $"Type: {levelTypeText}\n" +
                                $"Stars: {stars}/3";
        }
    }
    
    string GetLevelTypeText(int level)
    {
        if (level <= 10) return "Score Goal";
        if (level <= 20) return "Ice Blocks";
        if (level <= 30) return "Jewel Hunt";
        if (level <= 40) return "Time Bomb";
        return "Boss Level";
    }
    
    void PlaySelectedLevel()
    {
        if (selectedLevel > 0)
        {
            PlayerPrefs.SetInt("CurrentLevel", selectedLevel);
            PlayerPrefs.Save();
            
            // الانتقال إلى مشهد اللعبة
            SceneManager.LoadScene("GameScene");
        }
    }
    
    void GoBack()
    {
        // العودة إلى القائمة الرئيسية
        SceneManager.LoadScene("MainMenu");
    }
    
    // للاختبار: فتح جميع المراحل
    public void UnlockAllLevels()
    {
        ProgressManager.Instance.UnlockAllLevels(totalLevels);
        
        // تحديث العرض
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                int stars = ProgressManager.Instance.GetLevelStars(i + 1);
                levelButtons[i].Setup(i + 1, true, stars);
            }
        }
    }
}