using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 全局音频管理器。
/// 负责统一创建音源、播放 BGM/音效/UI 音频，并把玩家音量设置应用到运行时音源。
/// </summary>
public class AudioManager : MonoSingleton<AudioManager>
{
    #region 配置常量

    /// <summary>
    /// 普通音效音源池数量。
    /// </summary>
    private const int SfxSourceCount = 12;

    /// <summary>
    /// UI音效音源池数量。
    /// </summary>
    private const int UiSourceCount = 4;

    /// <summary>
    /// 3D空间音效音源池数量。
    /// </summary>
    private const int SpatialSourceCount = 8;

    #endregion

    #region 单例状态

    /// <summary>
    /// 当前运行中的全局音频管理器实例。
    /// 这里用于额外拦截场景中手动摆放的重复 AudioManager。
    /// </summary>
    private static AudioManager activeInstance;

    #endregion

    #region 音频缓存

    /// <summary>
    /// 已经通过 Addressables 加载过的 AudioClip 缓存。
    /// </summary>
    private readonly Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();

    /// <summary>
    /// 当前正在循环播放的非BGM音频。
    /// Key 为 AudioData.xlsx 中的 audioId，用于让业务层按稳定 ID 开始和停止持续音效。
    /// </summary>
    private readonly Dictionary<string, LoopingAudioRuntime> loopingAudioMap = new Dictionary<string, LoopingAudioRuntime>();

    #endregion

    #region 配置与加载器

    private IAssetLoader assetLoader;
    private AudioDataConfig audioDataConfig;

    #endregion

    #region 音源引用

    /// <summary>
    /// BGM专用音源。
    /// </summary>
    private AudioSource bgmSource;

    /// <summary>
    /// 普通音效音源池。
    /// </summary>
    private readonly List<AudioSource> sfxSources = new List<AudioSource>();

    /// <summary>
    /// UI音效音源池。
    /// </summary>
    private readonly List<AudioSource> uiSources = new List<AudioSource>();

    /// <summary>
    /// 3D空间音效音源池。
    /// </summary>
    private readonly List<AudioSource> spatialSources = new List<AudioSource>();

    /// <summary>
    /// 空间音效使用的 AudioMixerGroup，用于对接 Audio Reverb Zone。
    /// </summary>
    private AudioMixerGroup spatialMixerGroup;

    #endregion

    #region 运行时状态

    /// <summary>
    /// 当前是否已经完成初始化。
    /// </summary>
    private bool initialized;

    /// <summary>
    /// 当前正在播放的BGM Addressables Key。
    /// 用于避免重复播放同一首BGM。
    /// </summary>
    private string currentBgmKey;

    /// <summary>
    /// 当前BGM自身的音量倍率。
    /// 用于在玩家设置音量变化后，仍然保留 AudioData.xlsx 中配置的 defaultVolume。
    /// </summary>
    private float currentBgmVolumeScale = 1f;

    /// <summary>
    /// 当前BGM暂停请求数量。
    /// 视频、弹窗等多个入口同时请求暂停时，必须等全部归还后才恢复播放。
    /// </summary>
    private int bgmPauseRequestCount;

    /// <summary>
    /// 第一次暂停请求发生前，BGM是否处于播放状态。
    /// 用于避免原本没有BGM时，恢复阶段误启动BGM。
    /// </summary>
    private bool bgmWasPlayingBeforePause;

    #endregion

    #region 循环音效状态

    /// <summary>
    /// 单条循环音效的运行时状态。
    /// requestCount 用来支持多处同时请求同一个持续音效，最后一个请求结束后才真正停止。
    /// </summary>
    private class LoopingAudioRuntime
    {
        public AudioSource source;
        public AudioBus bus;
        public float volumeScale;
        public int requestCount;
    }

    #endregion

    #region 生命周期

    /// <summary>
    /// 场景加载后自动创建 AudioManager，避免从任意场景启动时缺少全局音频入口。
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        // 1. 访问 MonoSingleton 实例，触发查找或自动创建 AudioManager。
        Ins.Init();
    }

    private void Awake()
    {
        // 1. 如果已经存在全局实例，当前组件就是重复实例，只移除当前组件。
        if (activeInstance != null && activeInstance != this)
        {
            Destroy(this);
            return;
        }

        // 2. 记录全局实例，并完成自身初始化。
        activeInstance = this;
        Init();
    }

    private void OnDestroy()
    {
        // 1. 当前全局实例被销毁时，清空静态引用。
        if (activeInstance == this)
        {
            activeInstance = null;
        }
    }

    #endregion

    #region 初始化

    /// <summary>
    /// 初始化音频管理器。
    /// 会创建运行时音源，并把当前玩家设置中的音量应用到音源上。
    /// </summary>
    public void Init()
    {
        // 1. 防止重复初始化造成音源池重复创建。
        if (initialized)
        {
            return;
        }

        // 2. 先标记已初始化，避免后续 ApplySettingVolume 间接调用 Init 时递归创建音源。
        initialized = true;

        // 3. 音频管理器跨场景保留，保证BGM和音效入口稳定存在。
        DontDestroyOnLoad(gameObject);

        // 4. 创建内部运行时音源。
        CreateBgmSource();
        CreateSfxSources();
        CreateUiSources();
        LoadSpatialMixerGroup();
        CreateSpatialSources();

        // 5. 创建UI按钮点击音效自动绑定器。
        CreateUIButtonClickSfxBinder();

        // 6. 使用当前玩家设置刷新初始音量。
        ApplySettingVolume();

        // 6.5 加载音频配置表。
        LoadAudioDataConfig();

        // 6.6 创建资源加载器。
        CreateAssetLoader();

        // 7. 根据 AudioData.xlsx 中的 preload 配置预加载常用音频资源。
        PreloadConfiguredAudioClips();
    }

    /// <summary>
    /// 创建BGM专用音源。
    /// </summary>
    private void CreateBgmSource()
    {
        // 1. 创建BGM子物体，方便在层级面板中观察运行时结构。
        GameObject bgmObject = new GameObject("BGMSource");
        bgmObject.transform.SetParent(transform);

        // 2. 配置为2D循环音源，由 PlayBGM 主动指定 clip 后播放。
        bgmSource = bgmObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;
    }

    /// <summary>
    /// 创建普通音效音源池。
    /// </summary>
    private void CreateSfxSources()
    {
        // 1. 创建音效池根节点，统一挂在 AudioManager 下。
        GameObject root = new GameObject("SFXSources");
        root.transform.SetParent(transform);

        // 2. 创建多个2D短音效音源，支持多个音效同时播放。
        for (int i = 0; i < SfxSourceCount; i++)
        {
            AudioSource source = root.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            sfxSources.Add(source);
        }
    }

    /// <summary>
    /// 创建UI音效音源池。
    /// </summary>
    private void CreateUiSources()
    {
        // 1. 创建UI音效池根节点，让UI音效和游戏音效分组清晰。
        GameObject root = new GameObject("UISources");
        root.transform.SetParent(transform);

        // 2. UI音效也可能短时间内重叠播放，因此保留一个较小的池。
        for (int i = 0; i < UiSourceCount; i++)
        {
            AudioSource source = root.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            uiSources.Add(source);
        }
    }

    /// <summary>
    /// 创建UI按钮点击音效自动绑定器。
    /// </summary>
    private void CreateUIButtonClickSfxBinder()
    {
        // 1. 已经存在绑定器时直接复用，避免重复扫描按钮。
        if (GetComponent<UIButtonClickSfxBinder>() != null)
        {
            return;
        }

        // 2. 绑定器挂在 AudioManager 上，跟随全局音频入口跨场景保留。
        gameObject.AddComponent<UIButtonClickSfxBinder>();
    }

    private void LoadAudioDataConfig()
    {
        audioDataConfig = Resources.Load<AudioDataConfig>("AudioDataConfig");
        if (audioDataConfig == null)
        {
            Debug.LogWarning("AudioDataConfig.asset 未在 Resources 中找到，请在 Resources 目录下创建 AudioDataConfig ScriptableObject。");
        }
    }

    private void CreateAssetLoader()
    {
        assetLoader = new ResourcesAssetLoader();
    }

    /// <summary>
    /// 从 Resources 加载 AudioMixer 并获取空间音效 MixerGroup。
    /// </summary>
    private void LoadSpatialMixerGroup()
    {
        AudioMixer mixer = Resources.Load<AudioMixer>("Audio/MasterMixer");
        if (mixer == null)
        {
            return;
        }

        AudioMixerGroup[] groups = mixer.FindMatchingGroups("SpatialSFX");
        if (groups.Length > 0)
        {
            spatialMixerGroup = groups[0];
        }
    }

    /// <summary>
    /// 创建3D空间音效音源池。
    /// </summary>
    private void CreateSpatialSources()
    {
        GameObject root = new GameObject("SpatialSources");
        root.transform.SetParent(transform);

        for (int i = 0; i < SpatialSourceCount; i++)
        {
            AudioSource source = root.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1f;
            source.maxDistance = 50f;

            if (spatialMixerGroup != null)
            {
                source.outputAudioMixerGroup = spatialMixerGroup;
            }

            spatialSources.Add(source);
        }
    }

    #endregion

    #region BGM播放

    /// <summary>
    /// 播放指定BGM。
    /// </summary>
    /// <param name="addressableKey">BGM资源的 Addressables Key。</param>
    public void PlayBGM(string addressableKey)
    {
        PlayBGMInternal(addressableKey, 1f, true);
    }

    /// <summary>
    /// 播放指定BGM。
    /// </summary>
    /// <param name="addressableKey">BGM资源的 Addressables Key。</param>
    /// <param name="volumeScale">本次播放的音量倍率。</param>
    /// <param name="loop">是否循环播放。</param>
    private void PlayBGMInternal(string addressableKey, float volumeScale, bool loop)
    {
        // 1. 确保音频管理器已经完成初始化。
        Init();

        // 2. 空Key不参与播放，避免 Addressables 报错。
        if (string.IsNullOrEmpty(addressableKey))
        {
            return;
        }

        // 3. 同一首BGM已经在播放或被暂停时，不重复加载和重播。
        if (currentBgmKey == addressableKey && (bgmSource.isPlaying || bgmWasPlayingBeforePause))
        {
            currentBgmVolumeScale = Mathf.Clamp01(volumeScale);
            bgmSource.loop = loop;
            bgmSource.volume = GetRuntimeVolume(AudioBus.BGM, currentBgmVolumeScale);
            return;
        }

        // 4. 从缓存或 Addressables 中加载BGM资源。
        AudioClip clip = LoadClip(addressableKey, true);
        if (clip == null)
        {
            Debug.LogWarning("BGM load failed: " + addressableKey);
            return;
        }

        // 5. 切换当前BGM并按玩家设置音量和配置音量倍率播放。
        currentBgmKey = addressableKey;
        currentBgmVolumeScale = Mathf.Clamp01(volumeScale);
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = GetRuntimeVolume(AudioBus.BGM, currentBgmVolumeScale);
        bgmSource.Play();

        // 6. 如果当前存在视频等暂停请求，新切入的BGM也要立刻保持暂停。
        ApplyPendingBgmPause();
    }

    /// <summary>
    /// 停止当前BGM。
    /// </summary>
    public void StopBGM()
    {
        // 1. 确保BGM音源存在。
        Init();

        // 2. 清理当前BGM状态，避免下次同Key播放被误判为已播放。
        currentBgmKey = string.Empty;
        currentBgmVolumeScale = 1f;
        bgmPauseRequestCount = 0;
        bgmWasPlayingBeforePause = false;
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    /// <summary>
    /// 暂停当前BGM。
    /// 适合视频、教程等临时独占听觉焦点的流程调用。
    /// </summary>
    public void PauseBGM()
    {
        // 1. 确保BGM音源存在。
        Init();

        // 2. 记录暂停请求数量，支持多个视频入口叠加请求。
        bgmPauseRequestCount++;

        // 3. 只有第一条暂停请求真正操作音源，后续请求只增加计数。
        if (bgmPauseRequestCount > 1)
        {
            return;
        }

        // 4. 记录暂停前是否真的在播放，避免恢复时误启动空BGM。
        bgmWasPlayingBeforePause = bgmSource != null && bgmSource.isPlaying;
        if (!bgmWasPlayingBeforePause)
        {
            return;
        }

        // 5. 使用 Pause 保留播放进度，视频关闭后可以从原位置继续。
        bgmSource.Pause();
    }

    /// <summary>
    /// 恢复被 PauseBGM 暂停的BGM。
    /// </summary>
    public void ResumeBGM()
    {
        // 1. 确保BGM音源存在。
        Init();

        // 2. 没有暂停请求时忽略，防止外部重复恢复导致状态错乱。
        if (bgmPauseRequestCount <= 0)
        {
            bgmPauseRequestCount = 0;
            bgmWasPlayingBeforePause = false;
            return;
        }

        // 3. 归还一个暂停请求；仍有其他请求存在时保持暂停。
        bgmPauseRequestCount--;
        if (bgmPauseRequestCount > 0)
        {
            return;
        }

        // 4. 只有暂停前确实在播放，并且当前仍有 clip 时，才恢复播放。
        if (bgmWasPlayingBeforePause && bgmSource != null && bgmSource.clip != null)
        {
            bgmSource.UnPause();
        }

        // 5. 本轮暂停流程结束，清理恢复判断状态。
        bgmWasPlayingBeforePause = false;
    }

    /// <summary>
    /// 在已有暂停请求期间播放新BGM时，立即把新BGM也暂停住。
    /// </summary>
    private void ApplyPendingBgmPause()
    {
        // 1. 没有暂停请求或BGM并未真正播放时，无需处理。
        if (bgmPauseRequestCount <= 0 || bgmSource == null || !bgmSource.isPlaying)
        {
            return;
        }

        // 2. 新BGM已经开始播放，说明恢复阶段应该继续播放它。
        bgmWasPlayingBeforePause = true;
        bgmSource.Pause();
    }

    #endregion

    #region 配置音频播放

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 播放音频。
    /// 会根据音频自身的 bus 自动分发到 BGM、SFX、UI 或 Voice 通道。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="volumeScale">本次播放的额外音量倍率。</param>
    public void PlayAudioById(string audioId, float volumeScale = 1f)
    {
        // 1. 先从设计数据中读取音频配置，找不到时直接返回并给出警告。
        if (!TryGetAudioInfo(audioId, out AudioInfo audioInfo))
        {
            return;
        }

        // 2. 按配置中的 bus 分发播放通道，业务层不需要关心 Addressables Key。
        switch (audioInfo.bus)
        {
            case AudioBus.BGM:
                PlayConfiguredBGM(audioInfo, volumeScale);
                break;
            case AudioBus.SFX:
            case AudioBus.UI:
            case AudioBus.Voice:
                PlayConfiguredOneShot(audioInfo, volumeScale);
                break;
            default:
                Debug.LogWarning($"AudioData.xlsx 中 audioId 的 bus 不支持播放：{audioId} / {audioInfo.bus}");
                break;
        }
    }

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 播放BGM。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="volumeScale">本次播放的额外音量倍率。</param>
    public void PlayBGMById(string audioId, float volumeScale = 1f)
    {
        // 1. BGM 专用入口只接受 bus 为 BGM 的音频配置，避免表格分组填错后被静默播放。
        if (!TryGetAudioInfo(audioId, out AudioInfo audioInfo)
            || !ValidateAudioBus(audioInfo, AudioBus.BGM))
        {
            return;
        }

        PlayConfiguredBGM(audioInfo, volumeScale);
    }

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 播放普通游戏音效。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="volumeScale">本次播放的额外音量倍率。</param>
    public void PlaySFXById(string audioId, float volumeScale = 1f)
    {
        PlayOneShotById(audioId, AudioBus.SFX, volumeScale);
    }

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 播放UI音效。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="volumeScale">本次播放的额外音量倍率。</param>
    public void PlayUIById(string audioId, float volumeScale = 1f)
    {
        PlayOneShotById(audioId, AudioBus.UI, volumeScale);
    }

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 播放语音或讲解音频。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="volumeScale">本次播放的额外音量倍率。</param>
    public void PlayVoiceById(string audioId, float volumeScale = 1f)
    {
        PlayOneShotById(audioId, AudioBus.Voice, volumeScale);
    }

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 开始循环播放非BGM音频。
    /// 适合拖拽、持续喷射、持续加工这类需要手动停止的音效。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="volumeScale">本次播放的额外音量倍率。</param>
    public void StartLoopingAudioById(string audioId, float volumeScale = 1f)
    {
        // 1. 循环音效入口只负责非BGM通道，BGM仍然走 PlayBGMById。
        if (!TryGetAudioInfo(audioId, out AudioInfo audioInfo))
        {
            return;
        }

        if (audioInfo.bus == AudioBus.BGM)
        {
            Debug.LogWarning($"AudioData.xlsx 中 audioId 属于 BGM，不应通过循环音效入口播放：{audioId}");
            return;
        }

        StartConfiguredLoopingAudio(audioInfo, volumeScale);
    }

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 停止循环播放的非BGM音频。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="forceStop">是否忽略引用计数并立即停止所有同 ID 循环音效请求。</param>
    public void StopLoopingAudioById(string audioId, bool forceStop = false)
    {
        // 1. 空 ID 或当前没有循环播放时无需处理。
        if (string.IsNullOrWhiteSpace(audioId)
            || !loopingAudioMap.TryGetValue(audioId, out LoopingAudioRuntime runtime))
        {
            return;
        }

        // 2. 非强制停止时先归还一个请求；仍有其他请求存在则保持播放。
        if (!forceStop && runtime.requestCount > 1)
        {
            runtime.requestCount--;
            return;
        }

        // 3. 最后一个请求结束，或外部要求强制停止时，真正停止音源并释放循环占用。
        StopLoopingAudioInternal(audioId, runtime);
    }

    /// <summary>
    /// 尝试获取指定 audioId 的音频配置。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="audioInfo">匹配到的音频配置。</param>
    /// <returns>找到音频配置时返回 true。</returns>
    public bool TryGetAudioInfo(string audioId, out AudioInfo audioInfo)
    {
        // 1. 空 ID 无法查询配置，直接返回失败。
        if (string.IsNullOrWhiteSpace(audioId))
        {
            audioInfo = null;
            return false;
        }

        // 2. 通过设计数据入口查询 AudioData.xlsx 导出的音频配置。
        if (audioDataConfig != null && audioDataConfig.TryGetAudioInfo(audioId, out audioInfo))
        {
            return true;
        }

        audioInfo = null;
        Debug.LogWarning($"AudioData.xlsx 没有找到 audioId：{audioId}");
        return false;
    }

    /// <summary>
    /// 按指定分组播放配置表中的一次性音频。
    /// </summary>
    private void PlayOneShotById(string audioId, AudioBus expectedBus, float volumeScale)
    {
        // 1. 专用入口只接受对应 bus 的音频配置，帮助尽早发现表格分组错误。
        if (!TryGetAudioInfo(audioId, out AudioInfo audioInfo)
            || !ValidateAudioBus(audioInfo, expectedBus))
        {
            return;
        }

        PlayConfiguredOneShot(audioInfo, volumeScale);
    }

    /// <summary>
    /// 播放配置表中的BGM。
    /// </summary>
    private void PlayConfiguredBGM(AudioInfo audioInfo, float volumeScale)
    {
        // 1. 配置为空或 Key 缺失时不参与播放，保护调用方。
        if (audioInfo == null || string.IsNullOrWhiteSpace(audioInfo.addressableKey))
        {
            return;
        }

        // 2. defaultVolume 来自配置表，volumeScale 来自本次业务调用，两者相乘后参与最终音量。
        PlayBGMInternal(
            audioInfo.addressableKey,
            audioInfo.defaultVolume * volumeScale,
            audioInfo.loop);
    }

    /// <summary>
    /// 播放配置表中的一次性音频。
    /// </summary>
    private void PlayConfiguredOneShot(AudioInfo audioInfo, float volumeScale)
    {
        // 1. 配置为空或 Key 缺失时不参与播放，保护调用方。
        if (audioInfo == null || string.IsNullOrWhiteSpace(audioInfo.addressableKey))
        {
            return;
        }

        // 2. 一次性音频使用自身 bus 选择音源池，并叠加配置音量和本次调用音量。
        PlayOneShot(
            audioInfo.addressableKey,
            audioInfo.bus,
            audioInfo.defaultVolume * volumeScale);
    }

    /// <summary>
    /// 开始播放配置表中的循环音频。
    /// </summary>
    private void StartConfiguredLoopingAudio(AudioInfo audioInfo, float volumeScale)
    {
        // 1. 配置为空或 Key 缺失时不参与播放，保护调用方。
        if (audioInfo == null || string.IsNullOrWhiteSpace(audioInfo.addressableKey))
        {
            return;
        }

        float runtimeVolumeScale = Mathf.Clamp01(audioInfo.defaultVolume * volumeScale);

        // 2. 同一个 audioId 已经在循环播放时，只增加请求计数，避免重复叠音。
        if (loopingAudioMap.TryGetValue(audioInfo.audioId, out LoopingAudioRuntime runtime))
        {
            if (runtime == null || runtime.source == null)
            {
                loopingAudioMap.Remove(audioInfo.audioId);
            }
            else
            {
                runtime.requestCount++;
                runtime.volumeScale = Mathf.Max(runtime.volumeScale, runtimeVolumeScale);
                runtime.source.volume = GetRuntimeVolume(runtime.bus, runtime.volumeScale);

                if (!runtime.source.isPlaying)
                    runtime.source.Play();

                return;
            }
        }

        // 3. 从缓存或 Addressables 中加载音频资源。
        AudioClip clip = LoadClip(audioInfo.addressableKey, false);
        if (clip == null)
        {
            Debug.LogWarning("Looping audio clip load failed: " + audioInfo.addressableKey);
            return;
        }

        // 4. 取一个可用音源，设置为循环播放，并记录运行时状态，供后续停止和音量刷新使用。
        AudioSource source = GetFreeSource(audioInfo.bus);
        source.Stop();
        source.clip = clip;
        source.loop = true;
        source.volume = GetRuntimeVolume(audioInfo.bus, runtimeVolumeScale);
        source.Play();

        loopingAudioMap[audioInfo.audioId] = new LoopingAudioRuntime
        {
            source = source,
            bus = audioInfo.bus,
            volumeScale = runtimeVolumeScale,
            requestCount = 1
        };
    }

    /// <summary>
    /// 停止一条循环音效并清理它占用的音源。
    /// </summary>
    private void StopLoopingAudioInternal(string audioId, LoopingAudioRuntime runtime)
    {
        // 1. 先从循环表移除，避免后续 GetFreeSource 继续把这个音源视为循环音源。
        loopingAudioMap.Remove(audioId);

        // 2. 清理音源自身状态，让它重新回到普通音源池。
        if (runtime == null || runtime.source == null)
        {
            return;
        }

        runtime.source.Stop();
        runtime.source.clip = null;
        runtime.source.loop = false;
    }

    /// <summary>
    /// 检查音频配置是否属于预期播放分组。
    /// </summary>
    private bool ValidateAudioBus(AudioInfo audioInfo, AudioBus expectedBus)
    {
        // 1. 分组一致时允许播放。
        if (audioInfo != null && audioInfo.bus == expectedBus)
        {
            return true;
        }

        string audioId = audioInfo == null ? string.Empty : audioInfo.audioId;
        AudioBus actualBus = audioInfo == null ? default : audioInfo.bus;
        Debug.LogWarning($"AudioData.xlsx 中 audioId 的 bus 不匹配：{audioId}，当前为 {actualBus}，期望为 {expectedBus}");
        return false;
    }

    #endregion

    #region 一次性音效播放

    /// <summary>
    /// 播放普通游戏音效。
    /// </summary>
    /// <param name="addressableKey">音效资源的 Addressables Key。</param>
    /// <param name="volumeScale">本次播放的音量倍率。</param>
    public void PlaySFX(string addressableKey, float volumeScale = 1f)
    {
        PlayOneShot(addressableKey, AudioBus.SFX, volumeScale);
    }

    /// <summary>
    /// 播放UI音效。
    /// </summary>
    /// <param name="addressableKey">音效资源的 Addressables Key。</param>
    /// <param name="volumeScale">本次播放的音量倍率。</param>
    public void PlayUI(string addressableKey, float volumeScale = 1f)
    {
        PlayOneShot(addressableKey, AudioBus.UI, volumeScale);
    }

    /// <summary>
    /// 播放语音或讲解音频。
    /// 第一版暂时复用普通音效池，后续需要独立控制时再拆分专用音源。
    /// </summary>
    /// <param name="addressableKey">音频资源的 Addressables Key。</param>
    /// <param name="volumeScale">本次播放的音量倍率。</param>
    public void PlayVoice(string addressableKey, float volumeScale = 1f)
    {
        PlayOneShot(addressableKey, AudioBus.Voice, volumeScale);
    }

    /// <summary>
    /// 按指定音频分组播放一次性音效。
    /// </summary>
    private void PlayOneShot(string addressableKey, AudioBus bus, float volumeScale)
    {
        // 1. 确保音源池已经创建。
        Init();

        // 2. 空Key直接忽略，保护调用方。
        if (string.IsNullOrEmpty(addressableKey))
        {
            return;
        }

        // 3. 从缓存或 Addressables 中加载音频资源。
        AudioClip clip = LoadClip(addressableKey, false);
        if (clip == null)
        {
            Debug.LogWarning("Audio clip load failed: " + addressableKey);
            return;
        }

        // 4. 从对应音源池取一个可用音源，并按分组音量播放。
        AudioSource source = GetFreeSource(bus);
        source.volume = GetVolume(bus);
        source.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    #endregion

    #region 空间音频播放

    /// <summary>
    /// 在指定世界坐标播放普通游戏音效。
    /// 使用3D音源，支持 Audio Reverb Zone 混响效果。
    /// </summary>
    /// <param name="addressableKey">音效资源的 Addressables Key。</param>
    /// <param name="position">世界坐标位置。</param>
    /// <param name="volumeScale">本次播放的音量倍率。</param>
    public void PlaySpatialSFX(string addressableKey, Vector3 position, float volumeScale = 1f)
    {
        PlaySpatialOneShot(addressableKey, AudioBus.SFX, position, volumeScale);
    }

    /// <summary>
    /// 按 AudioData.xlsx 中配置的 audioId 在指定世界坐标播放空间音效。
    /// </summary>
    /// <param name="audioId">音频 ID。</param>
    /// <param name="position">世界坐标位置。</param>
    /// <param name="volumeScale">本次播放的额外音量倍率。</param>
    public void PlaySpatialAudioById(string audioId, Vector3 position, float volumeScale = 1f)
    {
        if (!TryGetAudioInfo(audioId, out AudioInfo audioInfo)
            || audioInfo == null
            || audioInfo.bus == AudioBus.BGM
            || audioInfo.bus == AudioBus.Voice)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(audioInfo.addressableKey))
        {
            return;
        }

        PlaySpatialOneShot(
            audioInfo.addressableKey,
            audioInfo.bus,
            position,
            audioInfo.defaultVolume * volumeScale);
    }

    /// <summary>
    /// 在指定世界坐标播放一次性空间音效。
    /// </summary>
    private void PlaySpatialOneShot(string addressableKey, AudioBus bus, Vector3 position, float volumeScale)
    {
        Init();

        if (string.IsNullOrEmpty(addressableKey))
        {
            return;
        }

        AudioClip clip = LoadClip(addressableKey, false);
        if (clip == null)
        {
            Debug.LogWarning("Spatial audio clip load failed: " + addressableKey);
            return;
        }

        AudioSource source = GetFreeSpatialSource();
        source.transform.position = position;
        source.volume = GetVolume(bus);
        source.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    /// <summary>
    /// 获取可用的3D空间音源。
    /// </summary>
    private AudioSource GetFreeSpatialSource()
    {
        for (int i = 0; i < spatialSources.Count; i++)
        {
            if (!spatialSources[i].isPlaying)
            {
                return spatialSources[i];
            }
        }

        return spatialSources[0];
    }

    /// <summary>
    /// 停止所有空间音效。
    /// </summary>
    public void StopAllSpatialAudio()
    {
        for (int i = 0; i < spatialSources.Count; i++)
        {
            if (spatialSources[i] != null)
            {
                spatialSources[i].Stop();
                spatialSources[i].clip = null;
            }
        }
    }

    /// <summary>
    /// 运行时设置空间音效的 AudioMixerGroup。
    /// 用于对接 Audio Reverb Zone 混响效果。
    /// </summary>
    public void SetSpatialMixerGroup(AudioMixerGroup mixerGroup)
    {
        spatialMixerGroup = mixerGroup;
        for (int i = 0; i < spatialSources.Count; i++)
        {
            if (spatialSources[i] != null)
            {
                spatialSources[i].outputAudioMixerGroup = mixerGroup;
            }
        }
    }

    #endregion

    #region 音频资源加载

    /// <summary>
    /// 预加载 AudioData.xlsx 中标记为 preload 的音频资源。
    /// </summary>
    public void PreloadConfiguredAudioClips()
    {
        // 1. 确保缓存字典和音源已经完成初始化。
        Init();

        if (audioDataConfig == null)
        {
            return;
        }

        List<AudioInfo> preloadAudioInfos = audioDataConfig.GetPreloadAudioInfos();

        // 2. 逐条加载标记为预加载的 AudioClip，LoadClip 内部会自动复用已有缓存。
        for (int i = 0; i < preloadAudioInfos.Count; i++)
        {
            AudioInfo audioInfo = preloadAudioInfos[i];

            if (audioInfo == null || string.IsNullOrWhiteSpace(audioInfo.addressableKey))
            {
                continue;
            }

            LoadClip(audioInfo.addressableKey, audioInfo.bus == AudioBus.BGM);
        }
    }

    /// <summary>
    /// 读取指定音频资源。
    /// 优先返回缓存中的 AudioClip；缓存不存在时走 IAssetLoader 加载。
    /// </summary>
    private AudioClip LoadClip(string addressableKey, bool isBgm)
    {
        // 1. 已加载过的资源直接复用，避免频繁同步加载。
        if (audioClipCache.TryGetValue(addressableKey, out AudioClip cachedClip))
        {
            return cachedClip;
        }

        // 2. BGM和普通音效通过 IAssetLoader 加载，默认使用 Resources 方式。
        AudioClip clip = assetLoader.LoadClip(addressableKey, isBgm);

        // 3. 加载成功后放入缓存，后续播放直接复用。
        if (clip != null)
        {
            audioClipCache[addressableKey] = clip;
        }

        return clip;
    }

    #endregion

    #region 音源池

    /// <summary>
    /// 获取指定音频分组可用的 AudioSource。
    /// </summary>
    private AudioSource GetFreeSource(AudioBus bus)
    {
        // 1. UI音效使用UI池，其余一次性音频先使用普通音效池。
        List<AudioSource> sources = bus == AudioBus.UI ? uiSources : sfxSources;

        // 2. 优先寻找当前没有播放内容、并且没有被循环音效占用的音源。
        for (int i = 0; i < sources.Count; i++)
        {
            if (!sources[i].isPlaying && !IsManagedLoopingSource(sources[i]))
            {
                return sources[i];
            }
        }

        // 3. 如果短音效全部繁忙，优先复用非循环音源，避免打断正在持续播放的循环音效。
        for (int i = 0; i < sources.Count; i++)
        {
            if (!IsManagedLoopingSource(sources[i]))
            {
                return sources[i];
            }
        }

        // 4. 音源全部被循环音效占用时兜底复用第一个音源，保证调用不会失败。
        return sources[0];
    }

    /// <summary>
    /// 判断指定音源当前是否被循环音效系统占用。
    /// </summary>
    private bool IsManagedLoopingSource(AudioSource source)
    {
        if (source == null || loopingAudioMap.Count == 0)
        {
            return false;
        }

        foreach (KeyValuePair<string, LoopingAudioRuntime> pair in loopingAudioMap)
        {
            LoopingAudioRuntime runtime = pair.Value;
            if (runtime != null && runtime.source == source)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region 音量控制

    /// <summary>
    /// 设置BGM音量。
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        SetVolume(AudioBus.BGM, volume);
    }

    /// <summary>
    /// 设置普通音效音量。
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        SetVolume(AudioBus.SFX, volume);
    }

    /// <summary>
    /// 设置UI音效音量。
    /// </summary>
    public void SetUIVolume(float volume)
    {
        SetVolume(AudioBus.UI, volume);
    }

    /// <summary>
    /// 设置指定音频分组的运行时音量。
    /// </summary>
    public void SetVolume(AudioBus bus, float volume)
    {
        // 1. 确保需要被设置的音源已经存在。
        Init();

        // 2. 音量统一限制在0到1之间，避免外部传入异常值。
        float clampedVolume = Mathf.Clamp01(volume);

        // 3. 根据音频分组写入对应音源或全局监听器音量。
        switch (bus)
        {
            case AudioBus.Master:
                AudioListener.volume = clampedVolume;
                break;
            case AudioBus.BGM:
                bgmSource.volume = Mathf.Clamp01(clampedVolume * currentBgmVolumeScale);
                break;
            case AudioBus.SFX:
                SetSourceListVolume(sfxSources, clampedVolume);
                RefreshLoopingAudioVolumes(AudioBus.SFX);
                break;
            case AudioBus.UI:
                SetSourceListVolume(uiSources, clampedVolume);
                RefreshLoopingAudioVolumes(AudioBus.UI);
                break;
            case AudioBus.Voice:
                SetSourceListVolume(sfxSources, clampedVolume);
                RefreshLoopingAudioVolumes(AudioBus.Voice);
                break;
        }
    }

    /// <summary>
    /// 获取指定音频分组当前应使用的音量。
    /// </summary>
    public float GetVolume(AudioBus bus)
    {
        // 1. 第一版音量数据直接映射到项目已有玩家设置数据。
        switch (bus)
        {
            case AudioBus.Master:
                return AudioListener.volume;
            case AudioBus.BGM:
                return Gaming_SettingData._Ins.bgmVolume;
            case AudioBus.SFX:
            case AudioBus.UI:
            case AudioBus.Voice:
                return Gaming_SettingData._Ins.audioVolume;
            default:
                return 1f;
        }
    }

    /// <summary>
    /// 获取指定音频分组本次播放时应使用的最终音量。
    /// </summary>
    private float GetRuntimeVolume(AudioBus bus, float volumeScale)
    {
        // 1. 玩家设置音量和单条音频配置音量相乘，最终仍限制在 0 到 1。
        return Mathf.Clamp01(GetVolume(bus) * Mathf.Clamp01(volumeScale));
    }

    /// <summary>
    /// 将当前玩家设置中的音量应用到所有运行时音源。
    /// </summary>
    public void ApplySettingVolume()
    {
        // 1. 总音量暂时保持最大值，BGM和音效分组跟随玩家设置。
        SetVolume(AudioBus.Master, 1f);

        // 2. BGM使用独立的bgmVolume。
        SetVolume(AudioBus.BGM, Gaming_SettingData._Ins.bgmVolume);

        // 3. 普通音效、UI音效、语音第一版都跟随audioVolume。
        SetVolume(AudioBus.SFX, Gaming_SettingData._Ins.audioVolume);
        SetVolume(AudioBus.UI, Gaming_SettingData._Ins.audioVolume);
        SetVolume(AudioBus.Voice, Gaming_SettingData._Ins.audioVolume);
    }

    /// <summary>
    /// 批量设置音源列表音量。
    /// </summary>
    private static void SetSourceListVolume(List<AudioSource> sources, float volume)
    {
        // 1. 遍历池中所有音源，保持同一分组音量一致。
        for (int i = 0; i < sources.Count; i++)
        {
            sources[i].volume = volume;
        }
    }

    /// <summary>
    /// 刷新指定分组中正在循环播放的音源音量。
    /// 循环音效需要保留 AudioData.xlsx 的 defaultVolume 和本次播放 volumeScale。
    /// </summary>
    private void RefreshLoopingAudioVolumes(AudioBus bus)
    {
        // 1. 没有循环音效时无需处理。
        if (loopingAudioMap.Count == 0)
        {
            return;
        }

        // 2. 只刷新目标分组，避免普通音效音量变化影响BGM等其他通道。
        foreach (KeyValuePair<string, LoopingAudioRuntime> pair in loopingAudioMap)
        {
            LoopingAudioRuntime runtime = pair.Value;
            if (runtime == null || runtime.source == null || runtime.bus != bus)
            {
                continue;
            }

            runtime.source.volume = GetRuntimeVolume(runtime.bus, runtime.volumeScale);
        }
    }

    #endregion
}
