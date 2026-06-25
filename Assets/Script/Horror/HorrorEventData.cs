using System;
using UnityEngine;

public enum HorrorEventType
{
    LightFlicker,
    DoorClose,
    Footstep,
    Whisper,
    ItemDrop,
    MirrorShadow
}

[Serializable]
public class HorrorEventData
{
    public string eventId;
    public HorrorEventType type;
    [Range(0f, 1f)]
    public float minIntensity;
    [Range(0f, 1f)]
    public float maxIntensity = 1f;
    public float cooldown = 5f;
    [Range(0f, 1f)]
    public float probability = 0.5f;
    public float duration = 2f;
}
