using UnityEngine;

public enum SpecialCellType
{
    None,
    Ice,
    Jewel,
    Bomb
}

public class GridCell : MonoBehaviour
{
    [Header("Cell State")]
    public int x;
    public int y;
    public bool isOccupied = false;
    public SpecialCellType specialType = SpecialCellType.None;
    
    [Header("Visual")]
    public SpriteRenderer normalRenderer;
    public SpriteRenderer iceRenderer;
    public SpriteRenderer jewelRenderer;
    public SpriteRenderer bombRenderer;
    public TextMesh bombText;
    
    [Header("Settings")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color occupiedColor = Color.white;
    public Color iceColor = new Color(0.7f, 0.9f, 1f, 0.8f);
    public Color jewelColor = new Color(1f, 0.8f, 0.2f, 1f);
    public Color bombColor = new Color(1f, 0.3f, 0.3f, 1f);
    
    private int bombMovesRemaining = 0;
    
    public void Initialize(int posX, int posY)
    {
        x = posX;
        y = posY;
        isOccupied = false;
        specialType = SpecialCellType.None;
        
        // إخفاء جميع العناصر البصرية
        HideAllVisuals();
        
        if (normalRenderer != null)
        {
            normalRenderer.color = normalColor;
        }
    }
    
    void HideAllVisuals()
    {
        if (iceRenderer != null) iceRenderer.gameObject.SetActive(false);
        if (jewelRenderer != null) jewelRenderer.gameObject.SetActive(false);
        if (bombRenderer != null) bombRenderer.gameObject.SetActive(false);
        if (bombText != null) bombText.gameObject.SetActive(false);
    }
    
    public void SetOccupied(Color color)
    {
        isOccupied = true;
        if (normalRenderer != null)
        {
            normalRenderer.color = color;
        }
    }
    
    public void SetIceBlock()
    {
        specialType = SpecialCellType.Ice;
        isOccupied = true;
        
        if (iceRenderer != null)
        {
            iceRenderer.gameObject.SetActive(true);
            iceRenderer.color = iceColor;
        }
    }
    
    public void SetJewel()
    {
        specialType = SpecialCellType.Jewel;
        isOccupied = true;
        
        if (jewelRenderer != null)
        {
            jewelRenderer.gameObject.SetActive(true);
            jewelRenderer.color = jewelColor;
        }
    }
    
    public void SetBomb(int moves)
    {
        specialType = SpecialCellType.Bomb;
        isOccupied = true;
        bombMovesRemaining = moves;
        
        if (bombRenderer != null)
        {
            bombRenderer.gameObject.SetActive(true);
            bombRenderer.color = bombColor;
        }
        
        UpdateBombText();
    }
    
    public void DecrementBomb()
    {
        if (specialType == SpecialCellType.Bomb)
        {
            bombMovesRemaining--;
            UpdateBombText();
            
            // تنبيه بصري عند انخفاض العدد
            if (bombMovesRemaining <= 3)
            {
                PulseEffect();
            }
        }
    }
    
    void UpdateBombText()
    {
        if (bombText != null)
        {
            bombText.text = bombMovesRemaining.ToString();
            bombText.gameObject.SetActive(true);
            
            if (bombMovesRemaining <= 3)
            {
                bombText.color = Color.red;
            }
        }
    }
    
    public bool IsBombExploded()
    {
        return specialType == SpecialCellType.Bomb && bombMovesRemaining <= 0;
    }
    
    public void Clear()
    {
        isOccupied = false;
        specialType = SpecialCellType.None;
        bombMovesRemaining = 0;
        
        HideAllVisuals();
        
        if (normalRenderer != null)
        {
            normalRenderer.color = normalColor;
        }
    }
    
    public void ClearIce()
    {
        if (specialType == SpecialCellType.Ice)
        {
            // إزالة الجليد فقط
            if (iceRenderer != null)
            {
                iceRenderer.gameObject.SetActive(false);
            }
            
            specialType = SpecialCellType.None;
            isOccupied = false;
            
            if (normalRenderer != null)
            {
                normalRenderer.color = normalColor;
            }
        }
    }
    
    public void CollectJewel()
    {
        if (specialType == SpecialCellType.Jewel)
        {
            // تأثير جمع الجوهرة
            PlayJewelEffect();
            
            Clear();
        }
    }
    
    void PlayJewelEffect()
    {
        // يمكن إضافة تأثير جسيمات هنا
        Debug.Log($"Jewel collected at ({x}, {y})!");
    }
    
    void PulseEffect()
    {
        // تنبيه بصري للقنبلة
        StartCoroutine(PulseCoroutine());
    }
    
    System.Collections.IEnumerator PulseCoroutine()
    {
        Vector3 originalScale = transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = 1f + Mathf.Sin(elapsed * Mathf.PI * 4) * 0.1f;
            transform.localScale = originalScale * scale;
            yield return null;
        }
        
        transform.localScale = originalScale;
    }
    
    public bool CanPlaceBlock()
    {
        return !isOccupied || specialType == SpecialCellType.Ice;
    }
}