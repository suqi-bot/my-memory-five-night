/// <summary>
/// 音频输出分组。
/// 用于让 AudioManager 判断音频应该走哪一类音量控制和播放通道。
/// </summary>
public enum AudioBus
{
    /// <summary>
    /// 总音量。
    /// </summary>
    Master,

    /// <summary>
    /// 背景音乐。
    /// </summary>
    BGM,

    /// <summary>
    /// 游戏内普通音效。
    /// </summary>
    SFX,

    /// <summary>
    /// UI按钮、弹窗等界面音效。
    /// </summary>
    UI,

    /// <summary>
    /// 语音、旁白、讲解等音频。
    /// </summary>
    Voice
}
