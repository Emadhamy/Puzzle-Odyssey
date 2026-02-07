using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScreenTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Optional Effects")]
    public bool useScaleEffect = false;
    public float scaleAmount = 1.1f;
    
    private bool isTransitioning = false;
    
    void Start()
    {
        // التأكد من وجود Image
        if (fadeImage == null)
        {
            // إنشاء Image إذا لم يكن موجوداً
            GameObject fadeObj = new GameObject("FadeImage");
            fadeObj.transform.SetParent(transform);
            fadeImage = fadeObj.AddComponent<Image>();
            fadeImage.color = Color.black;
            
            RectTransform rect = fadeImage.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }
        
        // البدء بشفافية كاملة ثم التلاشي
        fadeImage.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadeOut());
    }
    
    public void TransitionTo(Action onComplete)
    {
        if (isTransitioning) return;
        
        StartCoroutine(TransitionCoroutine(onComplete));
    }
    
    IEnumerator TransitionCoroutine(Action onComplete)
    {
        isTransitioning = true;
        
        // تلاشي للدخول (أسود)
        yield return StartCoroutine(FadeIn());
        
        // تنفيذ العملية
        onComplete?.Invoke();
        
        // انتظار لقطة واحدة
        yield return null;
        
        // تلاشي للخروج (شفاف)
        yield return StartCoroutine(FadeOut());
        
        isTransitioning = false;
    }
    
    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = fadeCurve.Evaluate(elapsed / fadeDuration);
            
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = color;
            
            yield return null;
        }
        
        fadeImage.color = new Color(0, 0, 0, 1);
    }
    
    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = fadeCurve.Evaluate(elapsed / fadeDuration);
            
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(1, 0, t);
            fadeImage.color = color;
            
            yield return null;
        }
        
        fadeImage.color = new Color(0, 0, 0, 0);
    }
    
    public void QuickFade(Action onComplete)
    {
        StartCoroutine(QuickFadeCoroutine(onComplete));
    }
    
    IEnumerator QuickFadeCoroutine(Action onComplete)
    {
        // تلاشي سريع
        float quickDuration = fadeDuration * 0.5f;
        float elapsed = 0f;
        
        while (elapsed < quickDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / quickDuration;
            
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0, 0.5f, t);
            fadeImage.color = color;
            
            yield return null;
        }
        
        onComplete?.Invoke();
        
        // عودة للشفافية
        elapsed = 0f;
        while (elapsed < quickDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / quickDuration;
            
            Color color = fadeImage.color;
            color.a = Mathf.Lerp(0.5f, 0, t);
            fadeImage.color = color;
            
            yield return null;
        }
        
        fadeImage.color = new Color(0, 0, 0, 0);
    }
}