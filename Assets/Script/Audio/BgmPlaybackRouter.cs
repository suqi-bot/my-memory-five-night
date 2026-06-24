/// <summary>
/// BGM业务播放入口。
/// 负责把当前游戏业务状态转换成 AudioData.xlsx 中的 audioId，避免各业务脚本散落硬编码。
/// </summary>
public static class BgmPlaybackRouter
{
    #region AudioId常量

    private const string MainBgmId = "bgm_main";

    #endregion

    #region 主界面BGM

    /// <summary>
    /// 播放首页、登录、主菜单使用的BGM。
    /// </summary>
    public static void PlayMain()
    {
        PlayBgm(MainBgmId);
    }

    #endregion

    #region 场景BGM

    /// <summary>
    /// 按场景名称播放BGM。
    /// 对应 AudioDataConfig 中配置为 bgm_{sceneName} 的条目。
    /// </summary>
    /// <param name="sceneName">场景名称。</param>
    public static void PlayBySceneName(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return;
        }

        PlayBgm("bgm_" + sceneName);
    }

    #endregion

    #region 通用播放

    /// <summary>
    /// 按 audioId 直接播放BGM。
    /// </summary>
    /// <param name="audioId">AudioDataConfig 中的音频ID。</param>
    public static void PlayById(string audioId)
    {
        PlayBgm(audioId);
    }

    /// <summary>
    /// 停止当前BGM。
    /// </summary>
    public static void StopBGM()
    {
        AudioManager.Ins.StopBGM();
    }

    #endregion

    #region 内部播放

    private static void PlayBgm(string audioId)
    {
        if (string.IsNullOrWhiteSpace(audioId))
        {
            return;
        }

        AudioManager.Ins.PlayBGMById(audioId);
    }

    #endregion
}
