using UnityEngine;

public class WhisperEvent : MonoBehaviour, IHorrorEvent
{
    public string EventId => "whisper";
    public bool IsActive { get; private set; }

    [SerializeField] private float whisperDuration = 3f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 10f;

    private float timer;
    private float intensity;

    public void Initialize(HorrorEventConfig config)
    {
    }

    public void Trigger(float intensity)
    {
        if (IsActive) return;

        this.intensity = intensity;
        IsActive = true;
        timer = 0;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            float distance = Random.Range(minDistance, maxDistance);
            Vector3 whisperPosition = mainCamera.transform.position + randomDirection * distance;

            AudioManager.Ins?.PlaySpatialAudioById("sfx_whisper", whisperPosition, intensity);
        }
    }

    public void Tick(float deltaTime)
    {
        if (!IsActive) return;

        timer += deltaTime;
        if (timer >= whisperDuration)
        {
            Stop();
        }
    }

    public void Stop()
    {
        IsActive = false;
    }
}
