# 恐怖氛围系统 (Horror Atmosphere System)

## 概述
恐怖氛围系统为《鬼屋试睡员》游戏提供沉浸式恐怖体验，包括环境音效、视觉效果和恐怖事件三大模块。系统支持游戏阶段控制、鬼怪接近度模拟和理智值管理。

## 系统架构

### 核心组件
1. **HorrorAtmosphereManager** - 主管理器，协调所有子系统
2. **AmbientAudioSystem** - 环境音效系统（低鸣、风声、心跳）
3. **HorrorVisualEffects** - 视觉效果系统（镜头晃动、屏幕闪烁、噪点）
4. **HorrorEventSystem** - 恐怖事件系统（灯光闪烁、门自动关闭、脚步声）

## 快速开始

### 1. 自动初始化（推荐）
在场景中添加 `HorrorSystemInitializer` 组件：
```csharp
// 挂载到任意GameObject上
gameObject.AddComponent<HorrorSystemInitializer>();
```

### 2. 手动初始化
```csharp
// 创建恐怖氛围系统
GameObject horrorSystem = new GameObject("HorrorSystem");
horrorSystem.AddComponent<HorrorAtmosphereManager>();
horrorSystem.AddComponent<AmbientAudioSystem>();
horrorSystem.AddComponent<HorrorVisualEffects>();
horrorSystem.AddComponent<HorrorEventSystem>();
```

### 3. 控制游戏阶段
```csharp
// 设置游戏阶段
HorrorAtmosphereManager.Ins.SetGamePhase(GamePhase.Dusk);      // 黄昏
HorrorAtmosphereManager.Ins.SetGamePhase(GamePhase.Midnight);   // 午夜
HorrorAtmosphereManager.Ins.SetGamePhase(GamePhase.Dawn);       // 黎明
```

### 4. 控制恐怖效果
```csharp
// 设置鬼怪接近度 (0-1)
HorrorAtmosphereManager.Ins.GhostProximity = 0.5f;

// 修改理智值
HorrorAtmosphereManager.Ins.ModifySanity(-10f);  // 减少10点
HorrorAtmosphereManager.Ins.ModifySanity(20f);   // 增加20点

// 触发惊吓效果
HorrorAtmosphereManager.Ins.TriggerJumpScare(0.8f);
```

## 游戏阶段说明

### Dusk（黄昏）- 收集阶段
- 氛围强度：0.2
- 轻微环境音效
- 偶尔的恐怖事件
- 玩家可以自由探索

### Midnight（午夜）- 防御阶段
- 氛围强度：1.0
- 强烈的环境音效
- 频繁的恐怖事件
- 鬼怪接近时心跳声增强

### Dawn（黎明）- 结束阶段
- 氛围强度：0
- 所有效果停止
- 游戏胜利

## 恐怖事件类型

| 事件类型 | 说明 | 触发条件 |
|---------|------|---------|
| LightFlicker | 灯光闪烁 | 氛围强度 > 0.1 |
| DoorClose | 门自动关闭 | 氛围强度 > 0.1 |
| Footstep | 远处脚步声 | 氛围强度 > 0.2 |
| Whisper | 低语声 | 氛围强度 > 0.6 |

## 音效配置

恐怖音效使用AudioDataConfig配置，需要创建以下音效：
- `ambient_hum` - 持续低频嗡嗡声
- `ambient_wind` - 风声
- `ambient_heartbeat` - 心跳声
- `sfx_door_close` - 门关闭声
- `sfx_footstep` - 脚步声
- `sfx_light_flicker` - 灯光闪烁声
- `sfx_whisper` - 低语声

**注意**：当前版本使用程序生成的占位音效。

## 测试模式

使用 `HorrorSystemExample` 组件进行测试：
- **F1**: 切换到黄昏阶段
- **F2**: 切换到午夜阶段
- **F3**: 切换到黎明阶段
- **J**: 触发惊吓效果

## 文件结构
```
Assets/Script/Horror/
├── HorrorAtmosphereManager.cs    # 主管理器
├── AmbientAudioSystem.cs         # 环境音效系统
├── HorrorVisualEffects.cs        # 视觉效果系统
├── HorrorEventSystem.cs          # 事件系统
├── HorrorEventConfig.cs          # 事件配置
├── HorrorEventData.cs            # 事件数据
├── IHorrorEvent.cs               # 事件接口
├── HorrorSystemInitializer.cs    # 初始化器
├── HorrorSystemExample.cs        # 示例脚本
└── Events/
    ├── LightFlickerEvent.cs      # 灯光闪烁事件
    ├── DoorCloseEvent.cs         # 门关闭事件
    ├── FootstepEvent.cs          # 脚步声事件
    └── WhisperEvent.cs           # 低语声事件
```

## 集成到现有代码

### 在游戏管理器中使用
```csharp
public class GameManager : MonoBehaviour
{
    private void Update()
    {
        // 根据游戏时间设置阶段
        if (gameTime >= 0 && gameTime < 6)
        {
            HorrorAtmosphereManager.Ins.SetGamePhase(GamePhase.Midnight);
        }
    }
}
```

### 在鬼怪AI中使用
```csharp
public class GhostAI : MonoBehaviour
{
    private void Update()
    {
        // 更新鬼怪接近度
        float distance = Vector3.Distance(transform.position, player.position);
        float proximity = 1f - Mathf.Clamp01(distance / maxDetectionRange);
        HorrorAtmosphereManager.Ins.GhostProximity = proximity;
    }
}
```

## 注意事项
1. 系统使用单例模式，确保场景中只有一个实例
2. 视觉效果不依赖Post Processing包，使用Unity内置方式实现
3. 占位音效会在最终版本替换为真实音效资源
4. 可以通过HorrorEventConfig自定义恐怖事件的触发概率和冷却时间