using UnityEngine;

public interface IAssetLoader
{
    AudioClip LoadClip(string key, bool isBgm);
}
