using UnityEngine;
using UnityEngine.UI;

public class HorrorVisualEffects : MonoBehaviour
{
    [Header("镜头晃动")]
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeFrequency = 20f;

    [Header("屏幕效果")]
    [SerializeField] private Canvas effectsCanvas;
    [SerializeField] private Image vignetteImage;
    [SerializeField] private Image flickerImage;
    [SerializeField] private RawImage noiseImage;

    [Header("效果参数")]
    [SerializeField] private float vignetteBaseAlpha = 0.3f;
    [SerializeField] private float flickerDuration = 0.1f;
    [SerializeField] private float noiseIntensity = 0.1f;

    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float currentShakeIntensity;
    private float shakeTimer;
    private float flickerTimer;
    private bool isFlickering;

    private Texture2D noiseTexture;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.localPosition;
        }

        CreateNoiseTexture();
        CreateEffectsUI();

        if (vignetteImage != null) vignetteImage.color = new Color(0, 0, 0, vignetteBaseAlpha);
        if (flickerImage != null) flickerImage.color = new Color(0, 0, 0, 0);
    }

    private void CreateNoiseTexture()
    {
        noiseTexture = new Texture2D(256, 256);
        Color[] pixels = new Color[256 * 256];
        for (int i = 0; i < pixels.Length; i++)
        {
            float value = Random.Range(0f, 1f);
            pixels[i] = new Color(value, value, value, 1f);
        }
        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();
    }

    private void CreateEffectsUI()
    {
        if (effectsCanvas != null) return;

        GameObject canvasObj = new GameObject("HorrorEffectsCanvas");
        canvasObj.transform.SetParent(transform);
        effectsCanvas = canvasObj.AddComponent<Canvas>();
        effectsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        effectsCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        CreateVignetteEffect(canvasObj.transform);
        CreateFlickerEffect(canvasObj.transform);
        CreateNoiseEffect(canvasObj.transform);
    }

    private void CreateVignetteEffect(Transform parent)
    {
        GameObject vignetteObj = new GameObject("Vignette");
        vignetteObj.transform.SetParent(parent, false);

        vignetteImage = vignetteObj.AddComponent<Image>();
        vignetteImage.color = new Color(0, 0, 0, vignetteBaseAlpha);

        RectTransform rect = vignetteObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    private void CreateFlickerEffect(Transform parent)
    {
        GameObject flickerObj = new GameObject("Flicker");
        flickerObj.transform.SetParent(parent, false);

        flickerImage = flickerObj.AddComponent<Image>();
        flickerImage.color = new Color(0, 0, 0, 0);
        flickerImage.raycastTarget = false;

        RectTransform rect = flickerObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    private void CreateNoiseEffect(Transform parent)
    {
        GameObject noiseObj = new GameObject("Noise");
        noiseObj.transform.SetParent(parent, false);

        noiseImage = noiseObj.AddComponent<RawImage>();
        noiseImage.texture = noiseTexture;
        noiseImage.color = new Color(1, 1, 1, noiseIntensity);
        noiseImage.raycastTarget = false;

        RectTransform rect = noiseObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    private void Update()
    {
        UpdateCameraShake();
        UpdateNoiseEffect();
    }

    public void UpdateEffects(float sanity, float ghostProximity)
    {
        float sanityFactor = 1f - (sanity / 100f);
        float intensity = Mathf.Max(sanityFactor, ghostProximity);

        UpdateVignette(intensity);
    }

    private void UpdateVignette(float intensity)
    {
        if (vignetteImage == null) return;

        float alpha = Mathf.Lerp(vignetteBaseAlpha, 0.8f, intensity);
        vignetteImage.color = new Color(0, 0, 0, alpha);
    }

    private void UpdateCameraShake()
    {
        if (mainCamera == null) return;

        if (currentShakeIntensity > 0)
        {
            float x = Mathf.Sin(Time.time * shakeFrequency) * currentShakeIntensity;
            float y = Mathf.Cos(Time.time * shakeFrequency * 1.3f) * currentShakeIntensity;
            mainCamera.transform.localPosition = originalCameraPosition + new Vector3(x, y, 0);

            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                currentShakeIntensity = 0;
                mainCamera.transform.localPosition = originalCameraPosition;
            }
        }
    }

    private void UpdateNoiseEffect()
    {
        if (noiseImage == null || noiseTexture == null) return;

        if (Time.frameCount % 3 == 0)
        {
            Color[] pixels = new Color[256 * 256];
            for (int i = 0; i < pixels.Length; i++)
            {
                float value = Random.Range(0f, 1f);
                pixels[i] = new Color(value, value, value, 1f);
            }
            noiseTexture.SetPixels(pixels);
            noiseTexture.Apply();
        }
    }

    public void TriggerCameraShake(float intensity, float duration)
    {
        currentShakeIntensity = intensity * shakeIntensity;
        shakeTimer = duration;
    }

    public void TriggerScreenFlash(float duration)
    {
        if (flickerImage == null) return;

        isFlickering = true;
        flickerTimer = duration;
        flickerImage.color = new Color(0, 0, 0, 0.8f);
        Invoke(nameof(EndFlicker), duration);
    }

    private void EndFlicker()
    {
        if (flickerImage != null)
        {
            flickerImage.color = new Color(0, 0, 0, 0);
        }
        isFlickering = false;
    }

    public void SetNoiseIntensity(float intensity)
    {
        noiseIntensity = intensity;
        if (noiseImage != null)
        {
            noiseImage.color = new Color(1, 1, 1, intensity);
        }
    }

    private void OnDestroy()
    {
        if (noiseTexture != null)
        {
            Destroy(noiseTexture);
        }
    }
}
