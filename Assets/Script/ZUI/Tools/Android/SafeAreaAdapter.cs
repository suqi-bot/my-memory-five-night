using UnityEngine;

public class SafeAreaAdapter : MonoBehaviour
{
    private void Start()
    {
        LimitSafeArea();
    }

    public void LimitSafeArea()
    {
        var rect = GetComponent<RectTransform>();
        LimitSafeArea(rect);
    }

    public void LimitSafeArea(RectTransform rect)
    {
        Debug.Log($"Screen: {Screen.width} - {Screen.height}");


        Rect safe = Screen.safeArea;

        Vector2 anchorMin = safe.position;
        Vector2 anchorMax = safe.position + safe.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;

        Debug.Log($"LimitSafeArea Applied: {anchorMin} - {anchorMax}");
    }
}