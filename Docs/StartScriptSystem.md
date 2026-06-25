# 启动脚本系统 (Start Script System)

## 概述
启动脚本系统是游戏的入口点，负责游戏的初始化和启动流程。系统在游戏开始时自动执行，初始化必要的游戏系统和UI。

## 系统架构

### 核心组件
1. **StartScript** - 启动脚本

## 快速开始

### 1. 启动脚本
```csharp
public class StartScript : MonoBehaviour
{
    void Start()
    {
        // 显示游戏场景面板
        UIManager.Instance.ShowPanel<GameSencePanel>();
    }
}
```

### 2. 初始化流程
```csharp
public class StartScript : MonoBehaviour
{
    void Start()
    {
        // 1. 初始化UI系统
        UIManager.Instance.ShowPanel<GameSencePanel>();
        
        // 2. 初始化音频系统
        AudioManager.Instance.Init();
        
        // 3. 初始化游戏数据
        GameDataManager.Instance.Init();
        
        // 4. 开始游戏
        StartGame();
    }
}
```

## 使用示例

### 完整启动流程
```csharp
public class StartScript : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    
    private void Start()
    {
        // 显示加载界面
        loadingPanel.SetActive(true);
        
        // 开始初始化
        StartCoroutine(InitializeGame());
    }
    
    private IEnumerator InitializeGame()
    {
        // 初始化各个系统
        yield return StartCoroutine(InitializeSystems());
        
        // 加载资源
        yield return StartCoroutine(LoadResources());
        
        // 隐藏加载界面
        loadingPanel.SetActive(false);
        
        // 显示游戏UI
        UIManager.Instance.ShowPanel<GameSencePanel>();
        
        // 开始游戏
        StartGame();
    }
    
    private IEnumerator InitializeSystems()
    {
        // 初始化UI系统
        progressBar.value = 0.2f;
        yield return new WaitForSeconds(0.1f);
        
        // 初始化音频系统
        progressBar.value = 0.4f;
        yield return new WaitForSeconds(0.1f);
        
        // 初始化游戏数据
        progressBar.value = 0.6f;
        yield return new WaitForSeconds(0.1f);
    }
    
    private IEnumerator LoadResources()
    {
        // 加载游戏资源
        progressBar.value = 0.8f;
        yield return new WaitForSeconds(0.1f);
        
        progressBar.value = 1.0f;
        yield return new WaitForSeconds(0.1f);
    }
    
    private void StartGame()
    {
        // 开始游戏逻辑
    }
}
```

## 文件结构
```
Assets/Script/
└── StartScript.cs    # 启动脚本
```

## 初始化流程

### 系统初始化顺序
1. UI系统初始化
2. 音频系统初始化
3. 游戏数据初始化
4. 场景加载
5. 游戏开始

### 自动初始化
```csharp
// 某些系统会自动初始化
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
private static void AutoCreate()
{
    // 自动创建音频管理器
    AudioManager.Instance.Init();
}
```

## 集成示例

### 与UI系统集成
```csharp
public class StartScript : MonoBehaviour
{
    void Start()
    {
        // 显示主菜单
        UIManager.Instance.ShowPanel<MainMenuPanel>();
    }
}
```

### 与事件系统集成
```csharp
public class StartScript : MonoBehaviour
{
    void Start()
    {
        // 触发游戏开始事件
        EventCenter.Instance.EventTrigger(E_EventType.GameStart);
    }
}
```

## 注意事项
1. 启动脚本需要挂载到场景中的GameObject上
2. 确保启动脚本在其他脚本之前执行
3. 避免在启动脚本中执行耗时操作
4. 使用协程处理异步初始化

## 扩展功能

### 场景管理
```csharp
public class SceneStartScript : MonoBehaviour
{
    void Start()
    {
        // 根据场景类型初始化
        string sceneName = SceneManager.GetActiveScene().name;
        
        switch (sceneName)
        {
            case "MainMenu":
                InitializeMainMenu();
                break;
            case "GameLevel":
                InitializeGameLevel();
                break;
        }
    }
}
```

### 配置加载
```csharp
public class ConfigStartScript : MonoBehaviour
{
    void Start()
    {
        // 加载游戏配置
        LoadGameConfig();
    }
    
    private void LoadGameConfig()
    {
        // 从文件加载配置
        string configPath = Application.persistentDataPath + "/config.json";
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            GameConfig config = JsonUtility.FromJson<GameConfig>(json);
            ApplyConfig(config);
        }
    }
}
```

## 最佳实践

### 启动脚本设计原则
1. 保持启动脚本简洁
2. 使用协程处理异步操作
3. 提供加载进度反馈
4. 处理初始化失败情况

### 性能优化
1. 避免在启动时加载过多资源
2. 使用异步加载避免卡顿
3. 合理安排初始化顺序
4. 使用对象池预创建常用对象