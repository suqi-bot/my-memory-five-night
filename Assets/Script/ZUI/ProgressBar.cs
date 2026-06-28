using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("UI")]
    public Image slider;   // Fill 类型 Image

    public Image icon;

    [Header("Time")]
    public float duration = 5f;   // 倒计时总时长（秒）

    float timer;
    bool isRunning;

    public Action onComplete;

    RectTransform sliderRect;
    RectTransform iconRect;

    void Awake()
    {
        CacheRectTransforms();
        UpdateIconPosition();
    }

    void OnEnable()
    {
        CacheRectTransforms();
        UpdateIconPosition();
    }

    void Update()
    {
        if (!isRunning)
        {
            UpdateIconPosition();
            return;
        }

        timer += Time.unscaledDeltaTime;

        float t = Mathf.Clamp01(timer / duration);
        SetProgress(1f - t);

        if (timer >= duration)
        {
            isRunning = false;
            SetProgress(0f);
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 开始倒计时
    /// </summary>
    public void StartCountdown(float time)
    {
        duration = Mathf.Max(0.01f, time);
        timer = 0f;
        isRunning = true;
        SetProgress(1f);
    }

    /// <summary>
    /// 停止并清空
    /// </summary>
    public void Stop()
    {
        isRunning = false;
        SetProgress(0f);
    }

    public void SetProgress(float progress)
    {
        if (slider == null) return;

        slider.fillAmount = Mathf.Clamp01(progress);
        UpdateIconPosition();
    }

    void CacheRectTransforms()
    {
        if (slider != null)
            sliderRect = slider.rectTransform;

        if (icon != null)
            iconRect = icon.rectTransform;
    }

    void UpdateIconPosition()
    {
        if (slider == null || icon == null) return;

        CacheRectTransforms();
        if (sliderRect == null || iconRect == null) return;

        Rect rect = sliderRect.rect;
        float progress = GetIconProgress(slider.fillAmount);
        Vector2 anchoredPosition = iconRect.anchoredPosition;
        anchoredPosition.x = Mathf.Lerp(rect.xMin, rect.xMax, progress);
        iconRect.anchoredPosition = anchoredPosition;
    }

    float GetIconProgress(float fillAmount)
    {
        fillAmount = Mathf.Clamp01(fillAmount);

        if (slider.fillMethod == Image.FillMethod.Horizontal &&
            slider.fillOrigin == (int)Image.OriginHorizontal.Right)
        {
            return 1f - fillAmount;
        }

        return fillAmount;
    }
}
