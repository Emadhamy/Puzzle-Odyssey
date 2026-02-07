using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button levelSelectButton;
    public Button settingsButton;
    public Button quitButton;
    
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    
    [Header("Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle vibrationToggle;
    public Button resetProgressButton;
    
    [Header("Info")]
    public TextMeshProUGUI versionText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI unlockedLevelsText;
    
    [Header("Transition")]
    public ScreenTransition screenTransition;
    
    void Start()
    {
        SetupButtons();
        LoadSettings();
        UpdateInfo();
    }
    
    void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(() => {
                int lastUnlocked = ProgressManager.Instance.GetUnlockedLevelsCount();
                PlayerPrefs.SetInt("CurrentLevel", lastUnlocked);
                screenTransition.TransitionTo(() => {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                });
            });
        }
        
        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.AddListener(() => {
                screenTransition.TransitionTo(() => {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect");
                });
            });
        }
        
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(() => {
                ToggleSettings();
            });
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => {
                Application.Quit();
            });
        }
        
        // Settings panel buttons
        if (resetProgressButton != null)
        {
            resetProgressButton.onClick.AddListener(() => {
                ResetProgress();
            });
        }
        
        // Sliders
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener((value) => {
                PlayerPrefs.SetFloat("MusicVolume", value);
                // AudioManager.Instance.SetMusicVolume(value);
            });
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener((value) => {
                PlayerPrefs.SetFloat("SFXVolume", value);
                // AudioManager.Instance.SetSFXVolume(value);
            });
        }
        
        if (vibrationToggle != null)
        {
            vibrationToggle.onValueChanged.AddListener((enabled) => {
                PlayerPrefs.SetInt("Vibration", enabled ? 1 : 0);
            });
        }
    }
    
    void LoadSettings()
    {
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
        
        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
        
        if (vibrationToggle != null)
        {
            vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        }
        
        if (versionText != null)
        {
            versionText.text = $"v{Application.version}";
        }
    }
    
    void UpdateInfo()
    {
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {ProgressManager.Instance.GetTotalScore()}";
        }
        
        if (unlockedLevelsText != null)
        {
            int unlocked = ProgressManager.Instance.GetUnlockedLevelsCount();
            unlockedLevelsText.text = $"Levels: {unlocked}/50";
        }
    }
    
    void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
    
    void ResetProgress()
    {
        // تأكيد من المستخدم
        // يمكن استخدام Dialog هنا
        
        ProgressManager.Instance.ResetAllProgress();
        UpdateInfo();
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}