using UnityEngine;

public class HorrorSystemExample : MonoBehaviour
{
    [Header("测试控制")]
    [SerializeField] private bool enableTestMode = true;
    [SerializeField] private GamePhase testPhase = GamePhase.Midnight;
    [SerializeField, Range(0f, 1f)] private float testGhostProximity = 0f;
    [SerializeField, Range(0f, 100f)] private float testSanity = 100f;

    [Header("按键绑定")]
    [SerializeField] private KeyCode jumpScareKey = KeyCode.J;
    [SerializeField] private KeyCode duskPhaseKey = KeyCode.F1;
    [SerializeField] private KeyCode midnightPhaseKey = KeyCode.F2;
    [SerializeField] private KeyCode dawnPhaseKey = KeyCode.F3;

    private HorrorAtmosphereManager atmosphereManager;

    private void Start()
    {
        atmosphereManager = HorrorAtmosphereManager.Ins;
    }

    private void Update()
    {
        if (!enableTestMode || atmosphereManager == null) return;

        if (Input.GetKeyDown(duskPhaseKey))
        {
            atmosphereManager.SetGamePhase(GamePhase.Dusk);
            Debug.Log("Phase set to: Dusk");
        }
        else if (Input.GetKeyDown(midnightPhaseKey))
        {
            atmosphereManager.SetGamePhase(GamePhase.Midnight);
            Debug.Log("Phase set to: Midnight");
        }
        else if (Input.GetKeyDown(dawnPhaseKey))
        {
            atmosphereManager.SetGamePhase(GamePhase.Dawn);
            Debug.Log("Phase set to: Dawn");
        }

        if (Input.GetKeyDown(jumpScareKey))
        {
            atmosphereManager.TriggerJumpScare(0.8f);
            Debug.Log("Jump scare triggered!");
        }

        atmosphereManager.GhostProximity = testGhostProximity;
        atmosphereManager.Sanity = testSanity;
    }

    private void OnGUI()
    {
        if (!enableTestMode) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== Horror System Test ===");
        GUILayout.Label($"Phase: {atmosphereManager?.CurrentPhase}");
        GUILayout.Label($"Atmosphere: {atmosphereManager?.AtmosphereIntensity:F2}");
        GUILayout.Label($"Sanity: {atmosphereManager?.Sanity:F1}");
        GUILayout.Label($"Ghost Proximity: {atmosphereManager?.GhostProximity:F2}");
        GUILayout.Space(10);
        GUILayout.Label("Controls:");
        GUILayout.Label("F1: Dusk | F2: Midnight | F3: Dawn");
        GUILayout.Label("J: Jump Scare");
        GUILayout.EndArea();
    }
}
