# 音频系统 (Audio System)

## 概述
音频系统为《鬼屋试睡员》游戏提供完整的音频管理功能，包括BGM播放、音效播放、UI音效、语音播放以及3D空间音效。系统支持音量控制、音频预加载和循环音效管理。

## 系统架构

### 核心组件
1. **AudioManager** - 全局音频管理器（单例模式）
2. **AudioDataConfig** - 音频配置数据（ScriptableObject）
3. **AudioInfo** - 音频信息数据类
4. **AudioBus** - 音频分组枚举（BGM、SFX、UI、Voice）
5. **IAssetLoader** - 资源加载器接口
6. **ResourcesAssetLoader** - Resources资源加载器实现

## 快速开始

### 1. 自动初始化
音频管理器会在场景加载后自动创建，无需手动初始化。

### 2. 播放BGM
```csharp
// 通过Addressables Key播放BGM
AudioManager.Instance.PlayBGM("bgm_main_theme");

// 通过配置ID播放BGM
AudioManager.Instance.PlayBGMById("bgm_main_theme");
```

### 3. 播放音效
```csharp
// 播放普通游戏音效
AudioManager.Instance.PlaySFX("sfx_door_open");

// 播放UI音效
AudioManager.Instance.PlayUI("sfx_button_click");

// 播放语音
AudioManager.Instance.PlayVoice("voice_narrator_01");
```

### 4. 播放3D空间音效
```csharp
// 在指定位置播放空间音效
Vector3 position = new Vector3(10, 0, 5);
AudioManager.Instance.PlaySpatialSFX("sfx_footstep", position);

// 通过配置ID播放空间音效
AudioManager.Instance.PlaySpatialAudioById("sfx_footstep", position);
```

### 5. 循环音效管理
```csharp
// 开始循环播放音效
AudioManager.Instance.StartLoopingAudioById("sfx_ambient_wind");

// 停止循环播放音效
AudioManager.Instance.StopLoopingAudioById("sfx_ambient_wind");

// 强制停止所有同ID循环音效
AudioManager.Instance.StopLoopingAudioById("sfx_ambient_wind", true);
```

## 音量控制

### 设置音量
```csharp
// 设置BGM音量（0-1）
AudioManager.Instance.SetBGMVolume(0.8f);

// 设置音效音量
AudioManager.Instance.SetSFXVolume(0.6f);

// 设置UI音效音量
AudioManager.Instance.SetUIVolume(0.7f);

// 设置主音量
AudioManager.Instance.SetVolume(AudioBus.Master, 1f);
```

### 获取音量
```csharp
// 获取当前BGM音量
float bgmVolume = AudioManager.Instance.GetVolume(AudioBus.BGM);

// 获取当前音效音量
float sfxVolume = AudioManager.Instance.GetVolume(AudioBus.SFX);
```

## BGM控制

### 暂停与恢复
```csharp
// 暂停BGM（支持多层暂停）
AudioManager.Instance.PauseBGM();

// 恢复BGM
AudioManager.Instance.ResumeBGM();

// 停止BGM
AudioManager.Instance.StopBGM();
```

## 音频配置

### AudioDataConfig配置
音频配置使用ScriptableObject，需要在Resources目录下创建`AudioDataConfig.asset`文件。

### 音频信息结构
```csharp
public class AudioInfo
{
    public string audioId;        // 音频ID
    public string addressableKey; // Addressables Key
    public AudioBus bus;          // 音频分组
    public float defaultVolume;   // 默认音量
    public bool loop;             // 是否循环
    public bool preload;          // 是否预加载
}
```

## 音频分组说明

| 分组 | 说明 | 音源池大小 |
|------|------|-----------|
| BGM | 背景音乐 | 1个专用音源 |
| SFX | 游戏音效 | 12个音源 |
| UI | 界面音效 | 4个音源 |
| Voice | 语音音频 | 复用SFX音源 |
| Spatial | 3D空间音效 | 8个音源 |

## 文件结构
```
Assets/Script/Audio/
├── AudioManager.cs           # 主管理器
├── AudioDataConfig.cs        # 音频配置
├── AudioInfo.cs              # 音频信息
├── AudioBus.cs               # 音频分组枚举
├── IAssetLoader.cs           # 资源加载器接口
├── ResourcesAssetLoader.cs   # Resources加载器
├── AddressableAssetLoader.cs # Addressables加载器
├── BgmPlaybackRouter.cs      # BGM播放路由
├── Gaming_SettingData.cs     # 游戏设置数据
├── UIButtonClickSfxBinder.cs # UI按钮音效绑定器
├── UIButtonClickSfxHandler.cs # UI按钮音效处理器
└── UIButtonClickSfxIgnore.cs # UI按钮音效忽略器
```

## 集成示例

### 在游戏管理器中使用
```csharp
public class GameManager : MonoBehaviour
{
    private void Start()
    {
        // 播放背景音乐
        AudioManager.Instance.PlayBGMById("bgm_main_theme");
    }

    public void OnGameOver()
    {
        // 停止BGM
        AudioManager.Instance.StopBGM();
        
        // 播放游戏结束音效
        AudioManager.Instance.PlaySFXById("sfx_game_over");
    }
}
```

### 在鬼怪AI中使用
```csharp
public class GhostAI : MonoBehaviour
{
    public void OnGhostAppear()
    {
        // 播放鬼怪出现音效
        AudioManager.Instance.PlaySpatialSFX("sfx_ghost_appear", transform.position);
    }
}
```

## 注意事项
1. 音频管理器使用单例模式，确保场景中只有一个实例
2. 音频管理器会自动跨场景保留（DontDestroyOnLoad）
3. 音频配置需要在Resources目录下创建AudioDataConfig.asset
4. 3D空间音效支持Audio Reverb Zone混响效果
5. 循环音效支持引用计数，多处同时请求时最后一个停止才真正停止