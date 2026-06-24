using UnityEngine;

public class AddressableAssetLoader : IAssetLoader
{
    public AudioClip LoadClip(string key, bool isBgm)
    {
        Debug.LogWarning("AddressableAssetLoader 尚未接入 Addressables 系统，请先安装 Addressables 包并实现加载逻辑。当前回退到静默忽略: " + key);
        return null;
    }
}
