using System.Collections.Generic;
using UnityEngine;

public class HorrorEventSystem : MonoBehaviour
{
    [SerializeField] private HorrorEventConfig eventConfig;

    private List<IHorrorEvent> registeredEvents = new List<IHorrorEvent>();
    private Dictionary<string, float> eventCooldowns = new Dictionary<string, float>();
    private float eventCheckTimer;
    private float eventCheckInterval = 2f;

    private void Start()
    {
        if (eventConfig == null)
        {
            eventConfig = Resources.Load<HorrorEventConfig>("HorrorEventConfig");
        }

        RegisterDefaultEvents();
    }

    private void RegisterDefaultEvents()
    {
        RegisterEvent(new LightFlickerEvent());
        RegisterEvent(new DoorCloseEvent());
        RegisterEvent(new FootstepEvent());
        RegisterEvent(new WhisperEvent());
    }

    public void RegisterEvent(IHorrorEvent horrorEvent)
    {
        if (eventConfig != null)
        {
            horrorEvent.Initialize(eventConfig);
        }
        registeredEvents.Add(horrorEvent);
    }

    public void UpdateEvents(float atmosphereIntensity)
    {
        UpdateCooldowns();

        eventCheckTimer += Time.deltaTime;
        if (eventCheckTimer >= eventCheckInterval)
        {
            eventCheckTimer = 0f;
            TryTriggerRandomEvent(atmosphereIntensity);
        }

        for (int i = 0; i < registeredEvents.Count; i++)
        {
            if (registeredEvents[i].IsActive)
            {
                registeredEvents[i].Tick(Time.deltaTime);
            }
        }
    }

    private void TryTriggerRandomEvent(float intensity)
    {
        if (eventConfig == null) return;

        List<HorrorEventData> availableEvents = eventConfig.GetEventsForIntensity(intensity);
        if (availableEvents.Count == 0) return;

        HorrorEventData selectedEvent = availableEvents[Random.Range(0, availableEvents.Count)];

        if (eventCooldowns.ContainsKey(selectedEvent.eventId) && eventCooldowns[selectedEvent.eventId] > 0)
        {
            return;
        }

        if (Random.Range(0f, 1f) > selectedEvent.probability)
        {
            return;
        }

        IHorrorEvent horrorEvent = FindEventByType(selectedEvent.type);
        if (horrorEvent != null && !horrorEvent.IsActive)
        {
            horrorEvent.Trigger(intensity);
            eventCooldowns[selectedEvent.eventId] = selectedEvent.cooldown;
        }
    }

    private IHorrorEvent FindEventByType(HorrorEventType type)
    {
        for (int i = 0; i < registeredEvents.Count; i++)
        {
            if (registeredEvents[i] is MonoBehaviour mb)
            {
                if (type == HorrorEventType.LightFlicker && mb is LightFlickerEvent)
                    return registeredEvents[i];
                if (type == HorrorEventType.DoorClose && mb is DoorCloseEvent)
                    return registeredEvents[i];
                if (type == HorrorEventType.Footstep && mb is FootstepEvent)
                    return registeredEvents[i];
                if (type == HorrorEventType.Whisper && mb is WhisperEvent)
                    return registeredEvents[i];
            }
        }
        return null;
    }

    private void UpdateCooldowns()
    {
        List<string> keys = new List<string>(eventCooldowns.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            eventCooldowns[keys[i]] -= Time.deltaTime;
            if (eventCooldowns[keys[i]] <= 0)
            {
                eventCooldowns.Remove(keys[i]);
            }
        }
    }

    public void StopAllEvents()
    {
        for (int i = 0; i < registeredEvents.Count; i++)
        {
            registeredEvents[i].Stop();
        }
    }
}
