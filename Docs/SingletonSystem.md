# 单例模式系统 (Singleton System)

## 概述
单例模式系统提供了两种单例实现：普通单例模式和MonoBehaviour单例模式。用于管理全局唯一的管理器实例，确保在整个应用程序生命周期中只有一个实例存在。

## 系统架构

### 核心组件
1. **BaseManager<T>** - 普通单例基类
2. **MonoSingleton<T>** - MonoBehaviour单例基类

## 快速开始

### 1. 普通单例
```csharp
// 继承BaseManager实现单例
public class GameManager : BaseManager<GameManager>
{
    public void Initialize()
    {
        // 初始化逻辑
    }
}

// 使用单例
GameManager.Instance.Initialize();
```

### 2. MonoBehaviour单例
```csharp
// 继承MonoSingleton实现单例
public class AudioManager : MonoSingleton<AudioManager>
{
    protected override void Awake()
    {
        base.Awake();
        // 初始化逻辑
    }
}

// 使用单例
AudioManager.Instance.PlayBGM("bgm_main");
```

## 使用示例

### 普通单例示例
```csharp
public class GameDataManager : BaseManager<GameDataManager>
{
    private Dictionary<string, object> data = new Dictionary<string, object>();
    
    public void SetData(string key, object value)
    {
        data[key] = value;
    }
    
    public T GetData<T>(string key)
    {
        if (data.ContainsKey(key))
        {
            return (T)data[key];
        }
        return default(T);
    }
}

// 使用
GameDataManager.Instance.SetData("playerName", "Player1");
string name = GameDataManager.Instance.GetData<string>("playerName");
```

### MonoBehaviour单例示例
```csharp
public class UIManager : MonoSingleton<UIManager>
{
    private Dictionary<string, UIPanel> panelDic = new Dictionary<string, UIPanel>();

    protected override void Awake()
    {
        base.Awake();
        // 初始化Canvas
        InitializeCanvas();
    }

    public T ShowPanel<T>() where T : UIPanel
    {
        // 显示面板逻辑
    }
}

// 使用
UIManager.Instance.ShowPanel<GameSencePanel>();
```

## 文件结构
```
Assets/Script/Singleton/
├── BaseManager.cs      # 普通单例基类
└── MonoSingleton.cs    # MonoBehaviour单例基类
```

## 单例模式详解

### 普通单例 (BaseManager)
```csharp
public class BaseManager<T> where T : new()
{
    private static T instance = new T();
    public static T Instance => instance;
}
```

**特点**:
- 线程安全（C#静态初始化）
- 自动创建实例
- 不依赖MonoBehaviour
- 适用于纯C#类

### MonoBehaviour单例 (MonoSingleton)
```csharp
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject singletonObj = new GameObject();
                    instance = singletonObj.AddComponent<T>();
                }
            }
            return instance;
        }
    }
    
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
```

**特点**:
- 继承MonoBehaviour
- 支持Unity生命周期
- 自动创建GameObject
- 防止重复实例

## 使用场景

### 普通单例适用场景
```csharp
// 游戏数据管理
public class GameDataManager : BaseManager<GameDataManager> { }

// 配置管理
public class ConfigManager : BaseManager<ConfigManager> { }

// 工具类
public class MathUtils : BaseManager<MathUtils> { }
```

### MonoBehaviour单例适用场景
```csharp
// 音频管理
public class AudioManager : MonoSingleton<AudioManager> { }

// UI管理
public class UIManager : MonoSingleton<UIManager> { }

// 场景管理
public class SceneManager : MonoSingleton<SceneManager> { }
```

## 集成示例

### 与事件系统集成
```csharp
public class EventManager : MonoSingleton<EventManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener(E_EventType.GameStart, OnGameStart);
    }
}
```

### 与音频系统集成
```csharp
public class AudioManager : MonoSingleton<AudioManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeAudioSources();
    }
    
    private void InitializeAudioSources()
    {
        // 初始化音源
    }
}
```

## 注意事项
1. 普通单例不能继承MonoBehaviour
2. MonoBehaviour单例需要确保场景中只有一个实例
3. 使用DontDestroyOnLoad保持跨场景存在
4. 避免在单例中存储过多状态
5. 注意单例的初始化顺序

## 扩展功能

### 延迟初始化
```csharp
public class LazySingleton<T> where T : class
{
    private static readonly Lazy<T> lazy = new Lazy<T>(() => Activator.CreateInstance<T>());
    public static T Instance => lazy.Value;
}
```

### 接口单例
```csharp
public interface ISingleton
{
    void Initialize();
}

public class Singleton<T> : ISingleton where T : class, ISingleton, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                instance.Initialize();
            }
            return instance;
        }
    }
}
```

## 最佳实践

### 设计原则
1. 单例应该只负责单一职责
2. 避免单例之间的循环依赖
3. 使用接口降低耦合
4. 合理控制单例的生命周期

### 性能优化
1. 避免在单例中频繁创建对象
2. 使用对象池管理频繁使用的对象
3. 合理使用DontDestroyOnLoad
4. 避免单例中存储大量数据

## 常见问题

### 单例为空
```csharp
// 确保单例已初始化
if (GameManager.Instance != null)
{
    GameManager.Instance.DoSomething();
}
```

### 重复实例
```csharp
// 在Awake中检查重复
protected virtual void Awake()
{
    if (instance == null)
    {
        instance = this as T;
    }
    else if (instance != this)
    {
        Destroy(gameObject);
    }
}
```

### 跨场景存在
```csharp
// 使用DontDestroyOnLoad
protected virtual void Awake()
{
    base.Awake();
    DontDestroyOnLoad(gameObject);
}
```