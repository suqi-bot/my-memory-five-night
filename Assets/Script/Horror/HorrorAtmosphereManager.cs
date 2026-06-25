using UnityEngine;

public enum GamePhase
{
    Dusk,
    Midnight,
    Dawn
}

public class HorrorAtmosphereManager : MonoSingleton<HorrorAtmosphereManager>
{
    [Header("氛围配置")]
    [SerializeField] private float duskIntensity = 0.2f;
    [SerializeField] private float midnightIntensity = 1.0f;
    [SerializeField] private float transitionSpeed = 0.5f;

    [Header("系统引用")]
    [SerializeField] private AmbientAudioSystem ambientAudio;
    [SerializeField] private HorrorVisualEffects visualEffects;
    [SerializeField] private HorrorEventSystem eventSystem;

    public float AtmosphereIntensity { get; private set; }
    public float GhostProximity { get; set; }
    public float Sanity { get; set; } = 100f;
    public GamePhase CurrentPhase { get; private set; } = GamePhase.Dusk;

    private float targetIntensity;
    private bool initialized;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
    {
        if (initialized) return;

        if (ambientAudio == null)
            ambientAudio = GetComponentInChildren<AmbientAudioSystem>();
        if (visualEffects == null)
            visualEffects = GetComponentInChildren<HorrorVisualEffects>();
        if (eventSystem == null)
            eventSystem = GetComponentInChildren<HorrorEventSystem>();

        Sanity = 100f;
        targetIntensity = duskIntensity;
        AtmosphereIntensity = duskIntensity;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        AtmosphereIntensity = Mathf.Lerp(AtmosphereIntensity, targetIntensity, Time.deltaTime * transitionSpeed);

        if (ambientAudio != null)
        {
            ambientAudio.UpdateAtmosphere(AtmosphereIntensity, GhostProximity);
        }

        if (visualEffects != null)
        {
            visualEffects.UpdateEffects(Sanity, GhostProximity);
        }

        if (eventSystem != null)
        {
            eventSystem.UpdateEvents(AtmosphereIntensity);
        }
    }

    public void SetGamePhase(GamePhase phase)
    {
        CurrentPhase = phase;
        switch (phase)
        {
            case GamePhase.Dusk:
                targetIntensity = duskIntensity;
                break;
            case GamePhase.Midnight:
                targetIntensity = midnightIntensity;
                break;
            case GamePhase.Dawn:
                targetIntensity = 0f;
                break;
        }
    }

    public void ModifySanity(float amount)
    {
        Sanity = Mathf.Clamp(Sanity + amount, 0f, 100f);
    }

    public void TriggerJumpScare(float intensity = 1f)
    {
        if (visualEffects != null)
        {
            visualEffects.TriggerCameraShake(intensity, 0.5f);
            visualEffects.TriggerScreenFlash(0.3f);
        }
    }
}
