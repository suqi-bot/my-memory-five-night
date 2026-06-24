using System;
using UnityEngine;

[Serializable]
public class AudioInfo
{
    public string audioId;
    public string addressableKey;
    public AudioBus bus;
    [Range(0f, 1f)] public float defaultVolume = 1f;
    public bool loop;
    public bool preload;
}
