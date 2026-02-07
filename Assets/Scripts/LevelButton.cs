using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI Elements")]
    public Button button;
    public TextMeshProUGUI levelNumberText;
    public Image lockIcon;
    public Image[] starImages;
    public Sprite starFilled;
    public Sprite starEmpty;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    
    [Header("Colors")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    
    private int levelNumber;
    private bool isUnlocked;
    
    public void Setup(int level, bool unlocked, int stars)
    {
        levelNumber = level;
        isUnlocked = unlocked;
        
        // عرض رقم المرحلة
        levelNumberText.text = level.ToString();
        
        // تحديث حالة القفل
        UpdateLockState(unlocked);
        
        // تحديث النجوم
        UpdateStars(stars);
        
        // إضافة مستمع للنقر
        button.onClick.RemoveAllListeners();
        if (unlocked)
        {
            button.onClick.AddListener(OnLevelClicked);
        }
    }
    
    void UpdateLockState(bool unlocked)
    {
        lockIcon.gameObject.SetActive(!unlocked);
        
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = unlocked ? unlockedSprite : lockedSprite;
            buttonImage.color = unlocked ? unlockedColor : lockedColor;
        }
        
        button.interactable = unlocked;
        levelNumberText.color = unlocked ? Color.black : Color.gray;
    }
    
    void UpdateStars(int stars)
    {
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
    }
    
    void OnLevelClicked()
    {
        Debug.Log($"Level {levelNumber} clicked!");
        LevelSelector.Instance.LoadLevel(levelNumber);
    }
}