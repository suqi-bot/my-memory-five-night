# 事件中心系统 (Event Center System)

## 概述
事件中心系统提供了一个全局的事件管理机制，用于实现游戏中的事件监听和触发。通过事件系统，可以实现模块间的解耦通信，提高代码的可维护性和可扩展性。

## 系统架构

### 核心组件
1. **EventCenter** - 事件中心管理器（单例模式）
2. **IEventInfo** - 事件信息接口
3. **EventInfo<T>** - 泛型事件信息类
4. **E_EventType** - 事件类型枚举

## 快速开始

### 1. 定义事件类型
```csharp
public enum E_EventType
{
    PlayerDeath,
    EnemyDeath,
    ScoreChanged,
    GameOver,
    LevelComplete
}
```

### 2. 监听事件
```csharp
// 监听无参数事件
EventCenter.Instance.AddEventListener(E_EventType.PlayerDeath, OnPlayerDeath);

// 监听带参数事件
EventCenter.Instance.AddEventListener<int>(E_EventType.ScoreChanged, OnScoreChanged);
```

### 3. 触发事件
```csharp
// 触发无参数事件
EventCenter.Instance.EventTrigger(E_EventType.PlayerDeath);

// 触发带参数事件
EventCenter.Instance.EventTrigger(E_EventType.ScoreChanged, 100);
```

### 4. 移除监听
```csharp
// 移除特定监听
EventCenter.Instance.RemoveEventListener(E_EventType.PlayerDeath, OnPlayerDeath);

// 移除所有监听
EventCenter.Instance.RemoveEventListener(E_EventType.PlayerDeath);

// 清空所有事件监听
EventCenter.Instance.ClearAllEventListener();
```

## 使用示例

### 玩家死亡事件
```csharp
public class Player : MonoBehaviour
{
    private void Die()
    {
        // 触发玩家死亡事件
        EventCenter.Instance.EventTrigger(E_EventType.PlayerDeath);
    }
}

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        // 监听玩家死亡事件
        EventCenter.Instance.AddEventListener(E_EventType.PlayerDeath, OnPlayerDeath);
    }
    
    private void OnDisable()
    {
        // 移除监听
        EventCenter.Instance.RemoveEventListener(E_EventType.PlayerDeath, OnPlayerDeath);
    }
    
    private void OnPlayerDeath()
    {
        // 处理玩家死亡逻辑
        Debug.Log("玩家死亡，游戏结束");
        GameOver();
    }
}
```

### 分数系统
```csharp
public class ScoreManager : MonoBehaviour
{
    private int score;
    
    public void AddScore(int points)
    {
        score += points;
        
        // 触发分数变化事件
        EventCenter.Instance.EventTrigger(E_EventType.ScoreChanged, score);
    }
}

public class ScoreUI : MonoBehaviour
{
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.ScoreChanged, UpdateScoreUI);
    }
    
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.ScoreChanged, UpdateScoreUI);
    }
    
    private void UpdateScoreUI(int newScore)
    {
        // 更新UI显示
        scoreText.text = "分数: " + newScore;
    }
}
```

## 事件类型说明

### 无参数事件
```csharp
// 定义
public enum E_EventType
{
    GameStart,
    GamePause,
    GameResume
}

// 监听
EventCenter.Instance.AddEventListener(E_EventType.GameStart, OnGameStart);

// 触发
EventCenter.Instance.EventTrigger(E_EventType.GameStart);
```

### 带参数事件
```csharp
// 监听带参数事件
EventCenter.Instance.AddEventListener<string>(E_EventType.Message, OnMessage);

// 触发带参数事件
EventCenter.Instance.EventTrigger(E_EventType.Message, "Hello World");
```

## 文件结构
```
Assets/Script/EventCenter/
└── EventCenter.cs    # 事件中心系统
```

## 工作原理

### 事件存储
- 使用Dictionary存储事件监听器
- Key为事件类型枚举
- Value为事件信息对象

### 事件触发流程
1. 查找事件类型对应的监听器
2. 调用所有注册的回调函数
3. 传递参数（如果有）

### 事件移除流程
1. 查找事件类型对应的监听器
2. 移除指定的回调函数
3. 如果回调为空，清空事件信息

## 性能优化

### 内存管理
- 及时移除不再需要的事件监听
- 避免在循环中频繁触发事件
- 使用对象池管理事件参数

### 使用建议
1. 在OnEnable中注册事件，在OnDisable中移除
2. 避免在事件回调中执行耗时操作
3. 合理设计事件类型，避免事件泛滥
4. 使用弱引用避免内存泄漏

## 注意事项
1. 事件类型枚举需要预先定义
2. 监听和触发的事件类型必须匹配
3. 带参数事件需要指定正确的参数类型
4. 避免循环触发事件导致栈溢出
5. 及时移除事件监听避免内存泄漏

## 扩展功能

### 事件优先级
```csharp
// 可以扩展事件系统支持优先级
public class EventInfoWithPriority<T> : IEventInfo
{
    public Dictionary<int, UnityAction<T>> priorityActions;
}
```

### 事件过滤
```csharp
// 可以扩展事件系统支持过滤
public interface IEventFilter
{
    bool ShouldTrigger(object data);
}
```

## 集成示例

### 与UI系统集成
```csharp
public class UIManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener(E_EventType.GameOver, ShowGameOverPanel);
    }
    
    private void ShowGameOverPanel()
    {
        ShowPanel<GameOverPanel>();
    }
}
```

### 与音频系统集成
```csharp
public class AudioManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener(E_EventType.PlayerDeath, PlayDeathSound);
    }
    
    private void PlayDeathSound()
    {
        PlaySFX("sfx_player_death");
    }
}
```