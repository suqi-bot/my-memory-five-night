# 恐怖氛围系统使用指南

## 目录
1. [系统概述](#系统概述)
2. [快速开始](#快速开始)
3. [核心组件详解](#核心组件详解)
4. [集成指南](#集成指南)
5. [配置说明](#配置说明)
6. [最佳实践建议](#最佳实践建议)
7. [常见问题](#常见问题)

---

## 系统概述

恐怖氛围系统为《鬼屋试睡员》提供沉浸式恐怖体验，包含以下核心模块：

| 模块 | 功能 | 文件 |
|------|------|------|
| **HorrorAtmosphereManager** | 主管理器，协调所有子系统 | `HorrorAtmosphereManager.cs` |
| **AmbientAudioSystem** | 环境音效（低鸣、风声、心跳） | `AmbientAudioSystem.cs` |
| **HorrorVisualEffects** | 视觉效果（镜头晃动、屏幕闪烁） | `HorrorVisualEffects.cs` |
| **HorrorEventSystem** | 恐怖事件（灯光闪烁、门关闭等） | `HorrorEventSystem.cs` |

---

## 快速开始

### 方式一：自动初始化（推荐）

1. 在场景中创建空物体，命名为 `HorrorSystem`
2. 添加 `HorrorSystemInitializer` 组件
3. 运行后自动创建所有子系统

```csharp
// HorrorSystemInitializer会自动创建并配置所有组件
// 无需手动添加其他脚本
```

### 方式二：手动初始化

```csharp
// 创建主容器
GameObject horrorSystem = new GameObject("HorrorSystem");

// 添加各个子系统
horrorSystem.AddComponent<HorrorAtmosphereManager>();
horrorSystem.AddComponent<AmbientAudioSystem>();
horrorSystem.AddComponent<HorrorVisualEffects>();
horrorSystem.AddComponent<HorrorEventSystem>();
```

---

## 核心组件详解

### 1. HorrorAtmosphereManager（主管理器）

**功能**：统一管理氛围强度、游戏阶段、理智值

**属性**：
```csharp
float AtmosphereIntensity  // 当前氛围强度 (0-1)
float GhostProximity       // 鬼怪接近度 (0-1)
float Sanity               // 理智值 (0-100)
GamePhase CurrentPhase     // 当前游戏阶段
```

**方法**：

```csharp
// 设置游戏阶段
void SetGamePhase(GamePhase phase)

// 修改理智值（正数增加，负数减少）
void ModifySanity(float amount)

// 触发惊吓效果（镜头晃动+屏幕闪烁）
void TriggerJumpScare(float intensity = 1f)
```

**使用示例**：
```csharp
public class GameManager : MonoBehaviour
{
    private void UpdateGamePhase(int hour)
    {
        switch (hour)
        {
            case 19: // 19:00 黄昏开始
                HorrorAtmosphereManager.Ins.SetGamePhase(GamePhase.Dusk);
                break;
            case 0:  // 00:00 午夜开始
                HorrorAtmosphereManager.Ins.SetGamePhase(GamePhase.Midnight);
                break;
            case 6:  // 06:00 黎明
                HorrorAtmosphereManager.Ins.SetGamePhase(GamePhase.Dawn);
                break;
        }
    }
}
```

---

### 2. AmbientAudioSystem（环境音效）

**功能**：管理背景音效，包括持续低鸣、随机风声、心跳声

**音效类型**：
| 音效 | 触发条件 | 说明 |
|------|---------|------|
| 低鸣 (Hum) | 系统启动 | 持续播放，营造压抑氛围 |
| 风声 (Wind) | 随机间隔 | 8-20秒随机触发 |
| 心跳 (Heartbeat) | 鬼怪接近 | 接近度>0.3时开始，渐强 |

**配置参数**（Inspector）：
```csharp
float windMinInterval = 8f      // 风声最小间隔
float windMaxInterval = 20f     // 风声最大间隔
float heartbeatMinProximity = 0.3f  // 心跳触发接近度
bool useProceduralAudio = true  // 使用程序生成音效
```

**使用示例**：
```csharp
// 系统会自动根据GhostProximity更新心跳声
// 无需手动调用，只需更新接近度
HorrorAtmosphereManager.Ins.GhostProximity = 0.7f;  // 心跳声会自动增强
```

---

### 3. HorrorVisualEffects（视觉效果）

**功能**：提供镜头晃动、屏幕闪烁、噪点、暗角等视觉恐怖效果

**效果类型**：
| 效果 | 触发方式 | 说明 |
|------|---------|------|
| 镜头晃动 | 手动触发/惊吓 | 随机偏移摄像机位置 |
| 屏幕闪烁 | 手动触发/惊吓 | 全屏黑色闪烁 |
| 噪点 | 持续显示 | 随机像素噪点 |
| 暗角 | 根据理智值 | 屏幕边缘变暗 |

**方法**：
```csharp
// 触发镜头晃动
void TriggerCameraShake(float intensity, float duration)

// 触发屏幕闪烁
void TriggerScreenFlash(float duration)

// 设置噪点强度
void SetNoiseIntensity(float intensity)
```

**使用示例**：
```csharp
public class PlayerDamage : MonoBehaviour
{
    public void OnPlayerHit()
    {
        // 受伤时触发视觉效果
        var visualEffects = FindObjectOfType<HorrorVisualEffects>();
        visualEffects.TriggerCameraShake(0.5f, 0.3f);
        visualEffects.TriggerScreenFlash(0.2f);
        
        // 降低理智值
        HorrorAtmosphereManager.Ins.ModifySanity(-10f);
    }
}
```

---

### 4. HorrorEventSystem（恐怖事件）

**功能**：随机触发各种恐怖事件，增强沉浸感

**事件类型**：
| 事件 | 说明 | 最低氛围强度 |
|------|------|-------------|
| LightFlicker | 灯光闪烁 | 0.1 |
| DoorClose | 门自动关闭 | 0.1 |
| Footstep | 远处脚步声 | 0.2 |
| Whisper | 低语声 | 0.6 |

**工作原理**：
1. 系统每2秒检查一次是否触发事件
2. 根据当前氛围强度筛选可用事件
3. 按配置概率决定是否触发
4. 触发后进入冷却时间

**自定义事件**：
```csharp
// 创建自定义事件
public class CustomHorrorEvent : MonoBehaviour, IHorrorEvent
{
    public string EventId => "custom_event";
    public bool IsActive { get; private set; }

    public void Initialize(HorrorEventConfig config) { }
    
    public void Trigger(float intensity)
    {
        if (IsActive) return;
        IsActive = true;
        // 自定义触发逻辑
    }
    
    public void Tick(float deltaTime)
    {
        if (!IsActive) return;
        // 自定义更新逻辑
    }
    
    public void Stop()
    {
        IsActive = false;
    }
}

// 注册自定义事件
var eventSystem = FindObjectOfType<HorrorEventSystem>();
eventSystem.RegisterEvent(new CustomHorrorEvent());
```

---

## 集成指南

### 集成到鬼怪AI

```csharp
public class GhostAI : MonoBehaviour
{
    [SerializeField] private float maxDetectionRange = 20f;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // 计算接近度
        float distance = Vector3.Distance(transform.position, player.position);
        float proximity = 1f - Mathf.Clamp01(distance / maxDetectionRange);
        
        // 更新恐怖系统
        HorrorAtmosphereManager.Ins.GhostProximity = proximity;
    }
}
```

### 集成到玩家状态

```csharp
public class PlayerSanity : MonoBehaviour
{
    [SerializeField] private float sanityDrainInDark = -2f;
    [SerializeField] private float sanityDrainNearGhost = -5f;
    
    private void Update()
    {
        // 黑暗中降低理智
        if (IsInDarkness())
        {
            HorrorAtmosphereManager.Ins.ModifySanity(sanityDrainInDark * Time.deltaTime);
        }
    }
    
    public void OnGhostSighted()
    {
        // 看到鬼怪时大幅降低理智
        HorrorAtmosphereManager.Ins.ModifySanity(sanityDrainNearGhost);
        
        // 触发惊吓
        HorrorAtmosphereManager.Ins.TriggerJumpScare(0.6f);
    }
}
```

### 集成到时间系统

```csharp
public class GameTimeManager : MonoBehaviour
{
    private float gameTime;
    private GamePhase currentPhase;
    
    private void Update()
    {
        gameTime += Time.deltaTime;
        
        // 根据游戏时间切换阶段
        GamePhase newPhase = CalculatePhase(gameTime);
        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            HorrorAtmosphereManager.Ins.SetGamePhase(currentPhase);
        }
    }
    
    private GamePhase CalculatePhase(float time)
    {
        // 19:00 - 00:00 = Dusk (收集阶段)
        // 00:00 - 06:00 = Midnight (防御阶段)
        // 06:00+ = Dawn (结束)
        if (gameTime < 5 * 60) return GamePhase.Dusk;      // 前5分钟
        if (gameTime < 15 * 60) return GamePhase.Midnight; // 中间10分钟
        return GamePhase.Dawn;                              // 最后
    }
}
```

---

## 配置说明

### HorrorEventConfig 配置

在Unity编辑器中创建配置文件：
1. 右键 `Assets/Resources`
2. `Create` → `Horror` → `Default Event Config`
3. 生成 `HorrorEventConfig.asset`

**配置参数说明**：
```csharp
[Serializable]
public class HorrorEventData
{
    public string eventId;          // 事件唯一标识
    public HorrorEventType type;    // 事件类型
    public float minIntensity;      // 触发所需最低氛围强度
    public float maxIntensity;      // 触发所需最高氛围强度
    public float cooldown;          // 冷却时间（秒）
    public float probability;       // 触发概率 (0-1)
    public float duration;          // 持续时间（秒）
}
```

**推荐配置**：
```csharp
// 黄昏阶段 - 轻微恐怖
{
    eventId = "light_flicker_dusk",
    type = HorrorEventType.LightFlicker,
    minIntensity = 0.1f,
    maxIntensity = 0.4f,
    cooldown = 15f,
    probability = 0.3f
}

// 午夜阶段 - 强烈恐怖
{
    eventId = "light_flicker_midnight",
    type = HorrorEventType.LightFlicker,
    minIntensity = 0.5f,
    maxIntensity = 1f,
    cooldown = 8f,
    probability = 0.6f
}
```

### AudioDataConfig 音频配置

**当前状态**：使用程序生成的占位音效

**替换为真实音效**：
1. 准备音效文件（.wav/.mp3）
2. 放置到 `Assets/Resources/Audio/Horror/`
3. 在 `AudioDataConfig.asset` 中配置

**所需音效列表**：
| 音效ID | 文件名 | 说明 |
|--------|--------|------|
| `ambient_hum` | ambient_hum.wav | 低频嗡嗡声 |
| `ambient_wind` | ambient_wind.wav | 风声 |
| `ambient_heartbeat` | ambient_heartbeat.wav | 心跳声 |
| `sfx_door_close` | sfx_door_close.wav | 门关闭声 |
| `sfx_footstep` | sfx_footstep.wav | 脚步声 |
| `sfx_light_flicker` | sfx_light_flicker.wav | 灯光闪烁声 |
| `sfx_whisper` | sfx_whisper.wav | 低语声 |

---

## 最佳实践建议

### 1. 阶段设计建议

| 阶段 | 氛围强度 | 事件频率 | 玩家体验 |
|------|---------|---------|---------|
| 黄昏 | 0.1-0.3 | 低 | 探索、收集、轻微紧张 |
| 午夜前期 | 0.4-0.6 | 中 | 逐渐紧张、开始防守 |
| 午夜后期 | 0.7-1.0 | 高 | 高压、频繁恐怖事件 |
| 黎明 | 0 | 无 | 释放、胜利感 |

### 2. 理智值设计建议

```csharp
// 理智值区间及效果
// 100-70: 正常状态
// 70-40: 轻微视觉干扰（噪点增加）
// 40-20: 明显视觉干扰（暗角、轻微扭曲）
// 20-0: 严重干扰（幻觉、Jump Scare频率增加）
```

### 3. 鬼怪接近度设计

```csharp
// 接近度区间及效果
// 0-0.3: 无明显效果
// 0.3-0.6: 心跳声开始
// 0.6-0.8: 心跳声增强，环境音变化
// 0.8-1.0: 最大紧张感，可能触发惊吓
```

### 4. 性能优化建议

1. **事件系统**：限制同时激活的事件数量
2. **视觉效果**：在低端设备上可禁用噪点效果
3. **音效系统**：使用对象池管理AudioSource
4. **灯光系统**：缓存场景灯光引用，避免每帧查找

### 5. 恐怖节奏设计

```csharp
// 推荐的恐怖节奏曲线
// 0-30秒: 平静期（让玩家适应）
// 30-60秒: 轻微事件（建立紧张感）
// 60-90秒: 事件频率增加
// 90-120秒: 高潮期（强烈恐怖事件）
// 120秒后: 循环或根据游戏进度调整
```

---

## 常见问题

### Q1: 系统没有自动初始化？

**解决方案**：
- 确保场景中有 `HorrorSystemInitializer` 组件
- 检查是否有多个实例（系统使用单例模式）
- 确认 `autoInitialize` 参数为 `true`

### Q2: 恐怖事件没有触发？

**检查项**：
1. 确认 `HorrorEventConfig` 已创建并配置
2. 检查事件的 `minIntensity` 是否小于当前氛围强度
3. 确认事件不在冷却时间内
4. 检查 `probability` 设置（0.3 = 30%概率）

### Q3: 音效没有播放？

**检查项**：
1. 确认 `AudioManager` 已初始化
2. 检查 `AudioDataConfig` 是否配置了对应音效ID
3. 确认音效文件已放置在正确路径
4. 检查音量设置是否为0

### Q4: 视觉效果不明显？

**调整建议**：
1. 增加 `shakeIntensity` 参数
2. 调整 `vignetteBaseAlpha` 值
3. 降低理智值以增强效果
4. 检查Canvas的Sorting Order是否正确

### Q5: 如何禁用特定效果？

```csharp
// 禁用环境音
var ambient = FindObjectOfType<AmbientAudioSystem>();
ambient.StopAll();

// 禁用视觉效果
var visual = FindObjectOfType<HorrorVisualEffects>();
visual.enabled = false;

// 禁用特定事件类型
// 在HorrorEventConfig中移除或设置probability为0
```

---

## 测试工具

### HorrorSystemExample 组件

用于开发阶段快速测试系统功能：

**快捷键**：
| 按键 | 功能 |
|------|------|
| F1 | 切换到黄昏阶段 |
| F2 | 切换到午夜阶段 |
| F3 | 切换到黎明阶段 |
| J | 触发惊吓效果 |

**使用方法**：
1. 添加 `HorrorSystemExample` 到任意物体
2. 运行游戏
3. 使用快捷键测试各阶段效果
4. 观察Inspector中的实时参数变化

---

## 更新日志

### v1.0 (当前版本)
- 核心氛围管理系统
- 程序生成占位音效
- 基础视觉效果
- 4种恐怖事件类型
- 配置系统框架

### 计划功能
- 更多恐怖事件类型
- 动态难度调整
- MOD支持

---

## 技术支持

如有问题或建议，请联系开发团队或查看源代码注释。

**关键文件路径**：
```
Assets/Script/Horror/
├── HorrorAtmosphereManager.cs
├── AmbientAudioSystem.cs
├── HorrorVisualEffects.cs
├── HorrorEventSystem.cs
├── HorrorEventConfig.cs
├── HorrorSystemInitializer.cs
└── Events/
    ├── LightFlickerEvent.cs
    ├── DoorCloseEvent.cs
    ├── FootstepEvent.cs
    └── WhisperEvent.cs
```
