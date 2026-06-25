using UnityEngine;

public class FootstepEvent : MonoBehaviour, IHorrorEvent
{
    public string EventId => "footstep";
    public bool IsActive { get; private set; }

    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private int footstepCount = 4;
    [SerializeField] private float footstepInterval = 0.6f;

    private float timer;
    private int currentStep;
    private Vector3 eventPosition;
    private float intensity;

    public void Initialize(HorrorEventConfig config)
    {
    }

    public void Trigger(float intensity)
    {
        if (IsActive) return;

        this.intensity = intensity;
        IsActive = true;
        currentStep = 0;
        timer = 0;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            float distance = Random.Range(minDistance, maxDistance);
            eventPosition = mainCamera.transform.position + randomDirection * distance;
        }

        PlayNextFootstep();
    }

    public void Tick(float deltaTime)
    {
        if (!IsActive) return;

        timer += deltaTime;
        if (timer >= footstepInterval)
        {
            timer = 0;
            currentStep++;

            if (currentStep >= footstepCount)
            {
                Stop();
                return;
            }

            PlayNextFootstep();
        }
    }

    private void PlayNextFootstep()
    {
        if (eventPosition == Vector3.zero) return;

        AudioManager.Ins?.PlaySpatialAudioById("sfx_footstep", eventPosition, intensity);

        Vector3 moveDirection = (Camera.main.transform.position - eventPosition).normalized;
        eventPosition += moveDirection * 0.5f;
    }

    public void Stop()
    {
        IsActive = false;
        currentStep = 0;
    }
}
