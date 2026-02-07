using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI jewelsText;
    public TextMeshProUGUI movesText;
    public GameObject goalPanel;
    public TextMeshProUGUI goalDescriptionText;
    
    [Header("Level Complete Screen")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI finalScoreText;
    public Image[] starImages;
    public Sprite starFilled;
    public Sprite starEmpty;
    public Button nextLevelButton;
    public Button retryButton;
    public Button levelSelectButton;
    
    [Header("Game Over Screen")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverScoreText;
    public Button gameOverRetryButton;
    public Button gameOverLevelSelectButton;
    
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button pauseRetryButton;
    public Button pauseLevelSelectButton;
    
    [Header("Screen Transitions")]
    public ScreenTransition screenTransition;
    
    void Start()
    {
        SetupButtons();
        HideAllPanels();
    }
    
    void SetupButtons()
    {
        // Level Complete buttons
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(() => {
                screenTransition.TransitionTo(() => LevelManager.Instance.NextLevel());
            });
        
        if (retryButton != null)
            retryButton.onClick.AddListener(() => {
                screenTransition.TransitionTo(() => LevelManager.Instance.RestartLevel());
            });
        
        if (levelSelectButton != null)
            levelSelectButton.onClick.AddListener(() => {
                screenTransition.TransitionTo(() => LevelManager.Instance.GoToLevelSelect());
            });
        
        // Game Over buttons
        if (gameOverRetryButton != null)
            gameOverRetryButton.onClick.AddListener(() => {
                screenTransition.TransitionTo(() => LevelManager.Instance.RestartLevel());
            });
        
        if (gameOverLevelSelectButton != null)
            gameOverLevelSelectButton.onClick.AddListener(() => {
                screenTransition.TransitionTo(() => LevelManager.Instance.GoToLevelSelect());
            });
        
        // Pause buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(() => LevelManager.Instance.PauseGame());
        
        if (pauseRetryButton != null)
            pauseRetryButton.onClick.AddListener(() => {
                LevelManager.Instance.PauseGame();
                LevelManager.Instance.RestartLevel();
            });
        
        if (pauseLevelSelectButton != null)
            pauseLevelSelectButton.onClick.AddListener(() => {
                LevelManager.Instance.PauseGame();
                LevelManager.Instance.GoToLevelSelect();
            });
    }
    
    void HideAllPanels()
    {
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }
    
    public void UpdateJewels(int current, int target)
    {
        if (jewelsText != null)
        {
            if (target > 0)
            {
                jewelsText.text = $"Jewels: {current}/{target}";
                jewelsText.gameObject.SetActive(true);
            }
            else
            {
                jewelsText.gameObject.SetActive(false);
            }
        }
    }
    
    public void UpdateMoves(int moves)
    {
        if (movesText != null)
        {
            movesText.text = $"Moves: {moves}";
        }
    }
    
    public void ShowGoal(string goalDescription)
    {
        if (goalPanel != null && goalDescriptionText != null)
        {
            goalDescriptionText.text = goalDescription;
            goalPanel.SetActive(true);
            
            // إخفاء بعد 3 ثواني
            Invoke(nameof(HideGoal), 3f);
        }
    }
    
    void HideGoal()
    {
        if (goalPanel != null)
        {
            goalPanel.SetActive(false);
        }
    }
    
    public void ShowLevelComplete(int score, int stars)
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Score: {score}";
            }
            
            // تحديث النجوم
            for (int i = 0; i < starImages.Length; i++)
            {
                if (i < stars)
                {
                    starImages[i].sprite = starFilled;
                    starImages[i].color = Color.yellow;
                }
                else
                {
                    starImages[i].sprite = starEmpty;
                    starImages[i].color = Color.gray;
                }
            }
            
            // تفعيل/تعطيل زر المرحلة التالية
            if (nextLevelButton != null)
            {
                int currentLevel = LevelManager.Instance.currentLevel;
                nextLevelButton.interactable = currentLevel < 50;
            }
            
            // تأثير النجوم
            StartCoroutine(StarAnimation(stars));
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = $"Final Score: {LevelManager.Instance.currentScore}";
            }
        }
    }
    
    public void ShowPauseMenu(bool show)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(show);
        }
    }
    
    System.Collections.IEnumerator StarAnimation(int stars)
    {
        // إخفاء جميع النجوم أولاً
        foreach (Image star in starImages)
        {
            star.transform.localScale = Vector3.zero;
        }
        
        // عرض النجوم واحدة تلو الأخرى
        for (int i = 0; i < stars; i++)
        {
            yield return new WaitForSeconds(0.3f);
            
            // تأثير التكبير
            float elapsed = 0f;
            float duration = 0.3f;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float scale = Mathf.Lerp(0f, 1.2f, t);
                starImages[i].transform.localScale = Vector3.one * scale;
                yield return null;
            }
            
            starImages[i].transform.localScale = Vector3.one;
            
            // صوت النجمة
            // AudioManager.Instance.PlayStarSound();
        }
    }
}