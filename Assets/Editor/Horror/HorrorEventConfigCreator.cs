using UnityEngine;
using UnityEditor;

public static class HorrorEventConfigCreator
{
    [MenuItem("Assets/Create/Horror/Default Event Config")]
    public static void CreateDefaultConfig()
    {
        HorrorEventConfig config = ScriptableObject.CreateInstance<HorrorEventConfig>();

        config.events.Add(new HorrorEventData
        {
            eventId = "light_flicker_dusk",
            type = HorrorEventType.LightFlicker,
            minIntensity = 0.1f,
            maxIntensity = 0.4f,
            cooldown = 15f,
            probability = 0.3f,
            duration = 2f
        });

        config.events.Add(new HorrorEventData
        {
            eventId = "light_flicker_midnight",
            type = HorrorEventType.LightFlicker,
            minIntensity = 0.5f,
            maxIntensity = 1f,
            cooldown = 8f,
            probability = 0.6f,
            duration = 3f
        });

        config.events.Add(new HorrorEventData
        {
            eventId = "door_close_dusk",
            type = HorrorEventType.DoorClose,
            minIntensity = 0.1f,
            maxIntensity = 0.3f,
            cooldown = 20f,
            probability = 0.2f,
            duration = 1.5f
        });

        config.events.Add(new HorrorEventData
        {
            eventId = "door_close_midnight",
            type = HorrorEventType.DoorClose,
            minIntensity = 0.4f,
            maxIntensity = 1f,
            cooldown = 10f,
            probability = 0.5f,
            duration = 1.5f
        });

        config.events.Add(new HorrorEventData
        {
            eventId = "footstep_dusk",
            type = HorrorEventType.Footstep,
            minIntensity = 0.2f,
            maxIntensity = 0.5f,
            cooldown = 12f,
            probability = 0.4f,
            duration = 3f
        });

        config.events.Add(new HorrorEventData
        {
            eventId = "footstep_midnight",
            type = HorrorEventType.Footstep,
            minIntensity = 0.5f,
            maxIntensity = 1f,
            cooldown = 6f,
            probability = 0.7f,
            duration = 4f
        });

        config.events.Add(new HorrorEventData
        {
            eventId = "whisper_midnight",
            type = HorrorEventType.Whisper,
            minIntensity = 0.6f,
            maxIntensity = 1f,
            cooldown = 10f,
            probability = 0.5f,
            duration = 3f
        });

        string path = "Assets/Resources/HorrorEventConfig.asset";
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;

        Debug.Log("HorrorEventConfig created at: " + path);
    }
}
