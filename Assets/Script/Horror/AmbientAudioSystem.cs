using UnityEngine;

public class AmbientAudioSystem : MonoBehaviour
{
    private const string AMBIENT_HUM = "ambient_hum";
    private const string AMBIENT_WIND = "ambient_wind";
    private const string AMBIENT_CLOCK = "ambient_clock";
    private const string AMBIENT_HEARTBEAT = "ambient_heartbeat";

    [Header("音效配置")]
    [SerializeField] private float windMinInterval = 8f;
    [SerializeField] private float windMaxInterval = 20f;
    [SerializeField] private float heartbeatMinProximity = 0.3f;

    [Header("占位音效生成")]
    [SerializeField] private bool useProceduralAudio = true;

    private float windTimer;
    private float nextWindTime;
    private bool heartbeatActive;
    private bool humActive;

    private AudioSource proceduralSource;

    private void Start()
    {
        if (useProceduralAudio)
        {
            CreateProceduralAudioSource();
        }

        StartAmbientHum();
        nextWindTime = Random.Range(windMinInterval, windMaxInterval);
    }

    private void CreateProceduralAudioSource()
    {
        proceduralSource = gameObject.AddComponent<AudioSource>();
        proceduralSource.loop = true;
        proceduralSource.playOnAwake = false;
        proceduralSource.spatialBlend = 0f;
    }

    public void UpdateAtmosphere(float intensity, float ghostProximity)
    {
        UpdateHeartbeat(ghostProximity);
        UpdateWindSound(intensity);
    }

    private void StartAmbientHum()
    {
        humActive = true;
        if (useProceduralAudio && proceduralSource != null)
        {
            PlayProceduralHum();
        }
        else
        {
            AudioManager.Ins?.StartLoopingAudioById(AMBIENT_HUM, 0.3f);
        }
    }

    private void UpdateHeartbeat(float proximity)
    {
        if (proximity >= heartbeatMinProximity)
        {
            if (!heartbeatActive)
            {
                heartbeatActive = true;
                if (useProceduralAudio)
                {
                    PlayProceduralHeartbeat();
                }
                else
                {
                    AudioManager.Ins?.StartLoopingAudioById(AMBIENT_HEARTBEAT, proximity);
                }
            }

            if (heartbeatActive && !useProceduralAudio)
            {
                AudioManager.Ins?.StopLoopingAudioById(AMBIENT_HEARTBEAT);
                AudioManager.Ins?.StartLoopingAudioById(AMBIENT_HEARTBEAT, proximity);
            }
        }
        else
        {
            if (heartbeatActive)
            {
                heartbeatActive = false;
                if (!useProceduralAudio)
                {
                    AudioManager.Ins?.StopLoopingAudioById(AMBIENT_HEARTBEAT);
                }
            }
        }
    }

    private void UpdateWindSound(float intensity)
    {
        windTimer += Time.deltaTime;
        if (windTimer >= nextWindTime)
        {
            windTimer = 0f;
            nextWindTime = Random.Range(windMinInterval, windMaxInterval) * (1f - intensity * 0.5f);

            if (useProceduralAudio)
            {
                PlayProceduralWind();
            }
            else
            {
                AudioManager.Ins?.PlaySFXById(AMBIENT_WIND, intensity);
            }
        }
    }

    private void PlayProceduralHum()
    {
        if (proceduralSource == null) return;

        int sampleRate = AudioSettings.outputSampleRate;
        int samples = sampleRate * 2;
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            data[i] = Mathf.Sin(2f * Mathf.PI * 60f * t) * 0.1f;
            data[i] += Mathf.Sin(2f * Mathf.PI * 120f * t) * 0.05f;
        }

        AudioClip clip = AudioClip.Create("ProceduralHum", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        proceduralSource.clip = clip;
        proceduralSource.volume = 0.2f;
        proceduralSource.Play();
    }

    private void PlayProceduralHeartbeat()
    {
        if (proceduralSource == null) return;

        int sampleRate = AudioSettings.outputSampleRate;
        int samples = sampleRate;
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float beat = Mathf.Exp(-10f * (t % 0.8f)) * Mathf.Sin(2f * Mathf.PI * 40f * t);
            data[i] = beat * 0.5f;
        }

        AudioClip clip = AudioClip.Create("ProceduralHeartbeat", samples, 1, sampleRate, false);
        clip.SetData(data, 0);

        GameObject heartbeatObj = new GameObject("HeartbeatSource");
        heartbeatObj.transform.SetParent(transform);
        AudioSource heartbeatSource = heartbeatObj.AddComponent<AudioSource>();
        heartbeatSource.clip = clip;
        heartbeatSource.loop = true;
        heartbeatSource.volume = 0.6f;
        heartbeatSource.Play();
    }

    private void PlayProceduralWind()
    {
        int sampleRate = AudioSettings.outputSampleRate;
        int samples = sampleRate;
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Sin(t * Mathf.PI) * 0.3f;
            data[i] = (Random.Range(-1f, 1f) * envelope);
        }

        AudioClip clip = AudioClip.Create("ProceduralWind", samples, 1, sampleRate, false);
        clip.SetData(data, 0);

        GameObject windObj = new GameObject("WindSource");
        windObj.transform.SetParent(transform);
        AudioSource windSource = windObj.AddComponent<AudioSource>();
        windSource.clip = clip;
        windSource.volume = 0.4f;
        windSource.Play();

        Destroy(windObj, 1.1f);
    }

    public void StopAll()
    {
        if (!useProceduralAudio)
        {
            AudioManager.Ins?.StopLoopingAudioById(AMBIENT_HUM);
            AudioManager.Ins?.StopLoopingAudioById(AMBIENT_HEARTBEAT);
        }

        heartbeatActive = false;
        humActive = false;

        foreach (Transform child in transform)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null)
            {
                source.Stop();
            }
        }
    }
}
