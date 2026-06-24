using UnityEngine;

public class ResourcesAssetLoader : IAssetLoader
{
    private const string AudioPathPrefix = "Audio/";

    public AudioClip LoadClip(string key, bool isBgm)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        return Resources.Load<AudioClip>(AudioPathPrefix + key);
    }
}
