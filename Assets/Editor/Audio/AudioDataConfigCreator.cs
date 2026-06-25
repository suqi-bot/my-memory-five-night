using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class AudioDataConfigCreator
{
    [MenuItem("Assets/Create/Audio/Default Audio Data Config")]
    public static void CreateDefaultConfig()
    {
        AudioDataConfig config = ScriptableObject.CreateInstance<AudioDataConfig>();

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "ambient_hum",
            addressableKey = "Audio/Horror/ambient_hum",
            bus = AudioBus.SFX,
            defaultVolume = 0.3f,
            loop = true,
            preload = true
        });

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "ambient_wind",
            addressableKey = "Audio/Horror/ambient_wind",
            bus = AudioBus.SFX,
            defaultVolume = 0.4f,
            loop = false,
            preload = true
        });

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "ambient_clock",
            addressableKey = "Audio/Horror/ambient_clock",
            bus = AudioBus.SFX,
            defaultVolume = 0.5f,
            loop = true,
            preload = false
        });

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "ambient_heartbeat",
            addressableKey = "Audio/Horror/ambient_heartbeat",
            bus = AudioBus.SFX,
            defaultVolume = 0.6f,
            loop = true,
            preload = true
        });

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "sfx_door_close",
            addressableKey = "Audio/Horror/sfx_door_close",
            bus = AudioBus.SFX,
            defaultVolume = 0.7f,
            loop = false,
            preload = true
        });

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "sfx_footstep",
            addressableKey = "Audio/Horror/sfx_footstep",
            bus = AudioBus.SFX,
            defaultVolume = 0.5f,
            loop = false,
            preload = true
        });

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "sfx_light_flicker",
            addressableKey = "Audio/Horror/sfx_light_flicker",
            bus = AudioBus.SFX,
            defaultVolume = 0.4f,
            loop = false,
            preload = false
        });

        config.audioInfos.Add(new AudioInfo
        {
            audioId = "sfx_whisper",
            addressableKey = "Audio/Horror/sfx_whisper",
            bus = AudioBus.SFX,
            defaultVolume = 0.3f,
            loop = false,
            preload = true
        });

        string path = "Assets/Resources/AudioDataConfig.asset";
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;

        Debug.Log("AudioDataConfig created at: " + path);
    }
}
