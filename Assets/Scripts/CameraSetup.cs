using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [Header("Camera Settings")]
    public float targetWidth = 10f;
    public float targetHeight = 12f;
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
    
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        // ضبط لون الخلفية
        cam.backgroundColor = backgroundColor;
        
        // ضبط حجم الكاميرا لإظهار الشبكة بشكل كامل
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = targetWidth / targetHeight;
        
        if (screenRatio >= targetRatio)
        {
            cam.orthographicSize = targetHeight / 2f;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            cam.orthographicSize = targetHeight / 2f * differenceInSize;
        }
        
        // وضع الكاميرا في المنتصف
        transform.position = new Vector3(0, 0, -10);
    }
}