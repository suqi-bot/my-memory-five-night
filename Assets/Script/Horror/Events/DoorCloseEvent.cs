using System.Collections.Generic;
using UnityEngine;

public class DoorCloseEvent : MonoBehaviour, IHorrorEvent
{
    public string EventId => "door_close";
    public bool IsActive { get; private set; }

    [SerializeField] private float closeSpeed = 2f;

    private List<Door> sceneDoors = new List<Door>();
    private Door targetDoor;
    private float timer;
    private float duration = 1.5f;

    public void Initialize(HorrorEventConfig config)
    {
        sceneDoors.AddRange(FindObjectsOfType<Door>());
    }

    public void Trigger(float intensity)
    {
        if (IsActive || sceneDoors.Count == 0) return;

        List<Door> openDoors = sceneDoors.FindAll(d => d != null);
        if (openDoors.Count == 0) return;

        targetDoor = openDoors[Random.Range(0, openDoors.Count)];
        IsActive = true;
        timer = 0;

        AudioManager.Ins?.PlaySFXById("sfx_door_close", intensity);
    }

    public void Tick(float deltaTime)
    {
        if (!IsActive || targetDoor == null) return;

        timer += deltaTime;
        if (timer >= duration)
        {
            Stop();
        }
    }

    public void Stop()
    {
        IsActive = false;
        targetDoor = null;
    }
}
