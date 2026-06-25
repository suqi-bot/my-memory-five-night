using UnityEngine;

public class HorrorSystemInitializer : MonoBehaviour
{
    [Header("自动初始化")]
    [SerializeField] private bool autoInitialize = true;

    [Header("配置引用")]
    [SerializeField] private HorrorEventConfig eventConfig;

    private HorrorAtmosphereManager atmosphereManager;

    private void Awake()
    {
        if (autoInitialize)
        {
            InitializeHorrorSystem();
        }
    }

    private void InitializeHorrorSystem()
    {
        if (HorrorAtmosphereManager.Ins == null)
        {
            GameObject horrorObj = new GameObject("HorrorAtmosphereSystem");
            atmosphereManager = horrorObj.AddComponent<HorrorAtmosphereManager>();

            GameObject ambientObj = new GameObject("AmbientAudioSystem");
            ambientObj.transform.SetParent(horrorObj.transform);
            ambientObj.AddComponent<AmbientAudioSystem>();

            GameObject visualObj = new GameObject("HorrorVisualEffects");
            visualObj.transform.SetParent(horrorObj.transform);
            visualObj.AddComponent<HorrorVisualEffects>();

            GameObject eventObj = new GameObject("HorrorEventSystem");
            eventObj.transform.SetParent(horrorObj.transform);
            HorrorEventSystem eventSystem = eventObj.AddComponent<HorrorEventSystem>();

            if (eventConfig != null)
            {
                eventSystem.GetType()
                    .GetField("eventConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(eventSystem, eventConfig);
            }

            DontDestroyOnLoad(horrorObj);
        }
        else
        {
            atmosphereManager = HorrorAtmosphereManager.Ins;
        }
    }

    public void SetGamePhase(GamePhase phase)
    {
        if (atmosphereManager != null)
        {
            atmosphereManager.SetGamePhase(phase);
        }
    }

    public void SetGhostProximity(float proximity)
    {
        if (atmosphereManager != null)
        {
            atmosphereManager.GhostProximity = proximity;
        }
    }

    public void ModifySanity(float amount)
    {
        if (atmosphereManager != null)
        {
            atmosphereManager.ModifySanity(amount);
        }
    }

    public void TriggerJumpScare(float intensity = 1f)
    {
        if (atmosphereManager != null)
        {
            atmosphereManager.TriggerJumpScare(intensity);
        }
    }
}
