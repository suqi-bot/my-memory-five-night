using System.Collections.Generic;
using UnityEngine;

public class LightFlickerEvent : MonoBehaviour, IHorrorEvent
{
    public string EventId => "light_flicker";
    public bool IsActive { get; private set; }

    [SerializeField] private float flickerSpeed = 10f;
    [SerializeField] private int flickerCount = 5;

    private List<Light> sceneLights = new List<Light>();
    private List<float> originalIntensities = new List<float>();
    private float timer;
    private int currentFlicker;
    private bool lightsOff;
    private float flickerTimer;

    public void Initialize(HorrorEventConfig config)
    {
        sceneLights.AddRange(FindObjectsOfType<Light>());
        for (int i = 0; i < sceneLights.Count; i++)
        {
            originalIntensities.Add(sceneLights[i].intensity);
        }
    }

    public void Trigger(float intensity)
    {
        if (IsActive || sceneLights.Count == 0) return;

        IsActive = true;
        currentFlicker = 0;
        flickerTimer = 0;
        lightsOff = false;

        AudioManager.Ins?.PlaySFXById("sfx_light_flicker", intensity);
    }

    public void Tick(float deltaTime)
    {
        if (!IsActive) return;

        flickerTimer += deltaTime;
        if (flickerTimer >= 1f / flickerSpeed)
        {
            flickerTimer = 0;
            lightsOff = !lightsOff;

            for (int i = 0; i < sceneLights.Count; i++)
            {
                if (sceneLights[i] != null)
                {
                    sceneLights[i].intensity = lightsOff ? 0 : originalIntensities[i];
                }
            }

            currentFlicker++;
            if (currentFlicker >= flickerCount * 2)
            {
                Stop();
            }
        }
    }

    public void Stop()
    {
        for (int i = 0; i < sceneLights.Count; i++)
        {
            if (sceneLights[i] != null)
            {
                sceneLights[i].intensity = originalIntensities[i];
            }
        }
        IsActive = false;
    }

    private void OnDestroy()
    {
        Stop();
    }
}
