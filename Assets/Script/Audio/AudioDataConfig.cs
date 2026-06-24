using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataConfig", menuName = "Audio/Audio Data Config")]
public class AudioDataConfig : ScriptableObject
{
    public List<AudioInfo> audioInfos = new List<AudioInfo>();

    private Dictionary<string, AudioInfo> lookup;

    public bool TryGetAudioInfo(string audioId, out AudioInfo audioInfo)
    {
        if (lookup == null)
        {
            BuildLookup();
        }

        return lookup.TryGetValue(audioId, out audioInfo);
    }

    public List<AudioInfo> GetPreloadAudioInfos()
    {
        List<AudioInfo> result = new List<AudioInfo>();
        for (int i = 0; i < audioInfos.Count; i++)
        {
            if (audioInfos[i] != null && audioInfos[i].preload)
            {
                result.Add(audioInfos[i]);
            }
        }
        return result;
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<string, AudioInfo>();
        for (int i = 0; i < audioInfos.Count; i++)
        {
            AudioInfo info = audioInfos[i];
            if (info != null && !string.IsNullOrWhiteSpace(info.audioId) && !lookup.ContainsKey(info.audioId))
            {
                lookup[info.audioId] = info;
            }
        }
    }

    private void OnValidate()
    {
        lookup = null;
    }
}
