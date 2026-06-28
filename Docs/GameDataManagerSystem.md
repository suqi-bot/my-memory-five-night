# 游戏数据管理系统 (Game Data Manager System)

## 概述
游戏数据管理系统负责管理游戏中的各种数据，包括玩家数据、游戏配置、存档数据等。系统提供统一的数据访问接口，支持数据的存储、读取和管理。

## 系统架构

### 核心组件
1. **GameDataManager** - 游戏数据管理器（单例模式）

## 快速开始

### 1. 访问数据管理器
```csharp
// 获取数据管理器实例
GameDataManager dataManager = GameDataManager.Instance;
```

### 2. 存储数据
```csharp
// 存储玩家数据
GameDataManager.Instance.SetData("playerName", "Player1");
GameDataManager.Instance.SetData("playerLevel", 10);
GameDataManager.Instance.SetData("playerScore", 1000);
```

### 3. 读取数据
```csharp
// 读取玩家数据
string playerName = GameDataManager.Instance.GetData<string>("playerName");
int playerLevel = GameDataManager.Instance.GetData<int>("playerLevel");
int playerScore = GameDataManager.Instance.GetData<int>("playerScore");
```

## 使用示例

### 玩家数据管理
```csharp
public class PlayerData
{
    public string playerName;
    public int level;
    public int score;
    public float health;
    public Vector3 position;
}

public class PlayerDataManager : MonoBehaviour
{
    private void SavePlayerData()
    {
        PlayerData data = new PlayerData
        {
            playerName = "Player1",
            level = 10,
            score = 1000,
            health = 100f,
            position = transform.position
        };
        
        GameDataManager.Instance.SetData("playerData", data);
    }
    
    private void LoadPlayerData()
    {
        PlayerData data = GameDataManager.Instance.GetData<PlayerData>("playerData");
        if (data != null)
        {
            // 应用数据
            transform.position = data.position;
        }
    }
}
```

### 游戏配置管理
```csharp
public class GameConfig
{
    public float musicVolume;
    public float sfxVolume;
    public int difficulty;
    public bool showTutorial;
}

public class ConfigManager : MonoBehaviour
{
    private void SaveConfig()
    {
        GameConfig config = new GameConfig
        {
            musicVolume = 0.8f,
            sfxVolume = 0.6f,
            difficulty = 2,
            showTutorial = true
        };
        
        GameDataManager.Instance.SetData("gameConfig", config);
    }
    
    private void LoadConfig()
    {
        GameConfig config = GameDataManager.Instance.GetData<GameConfig>("gameConfig");
        if (config != null)
        {
            // 应用配置
            AudioListener.volume = config.musicVolume;
        }
    }
}
```

## 文件结构
```
Assets/Script/
└── GameDataManager.cs    # 游戏数据管理器
```

## 数据管理功能

### 数据存储
```csharp
// 存储不同类型的数据
GameDataManager.Instance.SetData("stringData", "Hello");
GameDataManager.Instance.SetData("intData", 42);
GameDataManager.Instance.SetData("floatData", 3.14f);
GameDataManager.Instance.SetData("boolData", true);
GameDataManager.Instance.SetData("vector3Data", new Vector3(1, 2, 3));
```

### 数据读取
```csharp
// 读取数据
string str = GameDataManager.Instance.GetData<string>("stringData");
int num = GameDataManager.Instance.GetData<int>("intData");
float f = GameDataManager.Instance.GetData<float>("floatData");
bool b = GameDataManager.Instance.GetData<bool>("boolData");
Vector3 v = GameDataManager.Instance.GetData<Vector3>("vector3Data");
```

### 数据检查
```csharp
// 检查数据是否存在
if (GameDataManager.Instance.HasData("playerData"))
{
    // 数据存在
}

// 获取数据数量
int count = GameDataManager.Instance.GetDataCount();
```

## 数据持久化

### 保存到文件
```csharp
public void SaveToFile()
{
    // 序列化数据
    string json = JsonUtility.ToJson(GameDataManager.Instance.GetAllData());
    
    // 保存到文件
    string path = Application.persistentDataPath + "/save.json";
    File.WriteAllText(path, json);
}
```

### 从文件加载
```csharp
public void LoadFromFile()
{
    string path = Application.persistentDataPath + "/save.json";
    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        // 反序列化数据
        GameDataManager.Instance.LoadAllData(JsonUtility.FromJson<Dictionary<string, object>>(json));
    }
}
```

## 集成示例

### 与UI系统集成
```csharp
public class SettingsPanel : UIPanel
{
    public void OnSaveButtonClicked()
    {
        // 保存设置
        GameConfig config = new GameConfig
        {
            musicVolume = musicSlider.value,
            sfxVolume = sfxSlider.value
        };
        
        GameDataManager.Instance.SetData("gameConfig", config);
    }
    
    public void OnLoadButtonClicked()
    {
        // 加载设置
        GameConfig config = GameDataManager.Instance.GetData<GameConfig>("gameConfig");
        if (config != null)
        {
            musicSlider.value = config.musicVolume;
            sfxSlider.value = config.sfxVolume;
        }
    }
}
```

### 与事件系统集成
```csharp
public class GameDataManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener(E_EventType.GameStart, OnGameStart);
        EventCenter.Instance.AddEventListener(E_EventType.GameOver, OnGameOver);
    }
    
    private void OnGameStart()
    {
        // 游戏开始时加载数据
        LoadGameData();
    }
    
    private void OnGameOver()
    {
        // 游戏结束时保存数据
        SaveGameData();
    }
}
```

## 注意事项
1. 数据管理器使用单例模式
2. 数据类型需要支持序列化
3. 大量数据建议使用文件存储
4. 注意数据线程安全
5. 定期备份重要数据

## 扩展功能

### 数据加密
```csharp
public class SecureDataManager
{
    public void SetSecureData(string key, object value)
    {
        // 加密数据
        string encrypted = Encrypt(JsonUtility.ToJson(value));
        GameDataManager.Instance.SetData(key, encrypted);
    }
    
    public T GetSecureData<T>(string key)
    {
        // 解密数据
        string encrypted = GameDataManager.Instance.GetData<string>(key);
        string decrypted = Decrypt(encrypted);
        return JsonUtility.FromJson<T>(decrypted);
    }
}
```

### 数据验证
```csharp
public class DataValidator
{
    public bool ValidateData<T>(T data)
    {
        // 验证数据完整性
        return data != null;
    }
}
```

## 最佳实践

### 数据设计原则
1. 数据结构应该清晰明了
2. 使用强类型数据避免类型错误
3. 定期备份重要数据
4. 使用加密保护敏感数据

### 性能优化
1. 避免频繁读写大量数据
2. 使用缓存减少文件IO
3. 异步加载大数据
4. 压缩存储空间

## 常见问题

### 数据丢失
```csharp
// 定期保存数据
private void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus)
    {
        SaveGameData();
    }
}
```

### 数据类型错误
```csharp
// 使用强类型
public class PlayerData
{
    public string playerName;
    public int level;
}
```

### 数据同步问题
```csharp
// 使用锁确保线程安全
private readonly object dataLock = new object();

public void SetData(string key, object value)
{
    lock (dataLock)
    {
        // 设置数据
    }
}
```