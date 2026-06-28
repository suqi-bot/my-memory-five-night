# UI管理器系统 (UI Manager System)

## 概述
UI管理器系统负责管理游戏中的UI面板，包括面板的创建、显示、隐藏和销毁。系统支持面板的淡入淡出效果、面板缓存和UI层级管理。

## 系统架构

### 核心组件
1. **UIManager** - UI管理器（单例模式）
2. **UIPanel** - 面板基类
3. **EventManager** - 事件管理器

## 快速开始

### 1. 显示面板
```csharp
// 显示面板
UIManager.Instance.ShowPanel<GameSencePanel>();
```

### 2. 隐藏面板
```csharp
// 隐藏面板（带淡出效果）
UIManager.Instance.HidePanel<GameSencePanel>();

// 隐藏面板（无淡出效果）
UIManager.Instance.HidePanel<GameSencePanel>(false);
```

### 3. 获取面板
```csharp
// 获取已显示的面板
GameSencePanel panel = UIManager.Instance.GetPanel<GameSencePanel>();
```

### 4. 检查面板是否存在
```csharp
if (UIManager.Instance.HasPanel<GameSencePanel>())
{
    // 面板存在
}
```

### 5. 隐藏所有面板
```csharp
UIManager.Instance.HideAllPanels();
```

## 使用示例

### 面板管理
```csharp
public class UIManager
{
    public static UIManager Instance => instance;
    private static UIManager instance = new UIManager();

    private Dictionary<string, UIPanel> panelDic = new Dictionary<string, UIPanel>();
    private Transform canvasTrans;

    private UIManager()
    {
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvas.transform;
        GameObject.DontDestroyOnLoad(canvas);
    }

    public T ShowPanel<T>() where T : UIPanel
    {
        string panelName = typeof(T).Name;

        // 检查缓存
        if (panelDic.ContainsKey(panelName))
        {
            return (T)panelDic[panelName];
        }

        // 创建面板
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName), canvasTrans);
        T panel = panelObj.GetComponent<T>();
        panel.Show();
        panelDic.Add(panelName, panel);

        return panel;
    }

    public void HidePanel<T>(bool isFade = true) where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (!panelDic.ContainsKey(panelName)) return;

        T panel = (T)panelDic[panelName];
        if (isFade)
        {
            panel.Hide(() =>
            {
                GameObject.Destroy(panel.gameObject);
                panelDic.Remove(panelName);
            });
        }
        else
        {
            panel.Hide();
            GameObject.Destroy(panel.gameObject);
            panelDic.Remove(panelName);
        }
    }

    public T GetPanel<T>() where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return (T)panelDic[panelName];
        return null;
    }

    public bool HasPanel<T>() where T : UIPanel
    {
        return panelDic.ContainsKey(typeof(T).Name);
    }

    public void HideAllPanels()
    {
        foreach (var panel in panelDic.Values)
        {
            if (panel != null)
                GameObject.Destroy(panel.gameObject);
        }
        panelDic.Clear();
    }
}
```

### 面板生命周期
```csharp
public class GameSencePanel : UIPanel
{
    public Image enduranceBG;
    public Image enduranceFG;

    public override void Init(object data = null)
    {
        base.Init(data);
        // 面板初始化
    }

    public override void Show()
    {
        base.Show();
        // 面板显示时的逻辑
    }

    public override void Hide()
    {
        base.Hide();
        // 面板隐藏时的逻辑
    }

    public override void ResetPanel()
    {
        base.ResetPanel();
        // 重置面板状态
    }

    public void ChangeEnduranceValue(float max, float now)
    {
        enduranceFG.rectTransform.sizeDelta = new Vector2(
            enduranceBG.rectTransform.sizeDelta.x * now / max,
            enduranceFG.rectTransform.sizeDelta.y
        );
    }
}
```

## 文件结构
```
Assets/Script/ZUI/
├── UIPanel.cs           # 面板基类
├── UIManager.cs         # UI管理器
├── PanelBtn.cs          # 面板按钮
└── ...

Assets/Script/UI/
└── GameSencePanel.cs    # 游戏场景面板

Assets/Script/EventCenter/
├── EventManager.cs      # 事件管理器
└── EventType.cs         # 事件类型
```

## 面板管理功能

### 面板缓存
```csharp
// 自动缓存已创建的面板
private Dictionary<string, UIPanel> panelDic = new Dictionary<string, UIPanel>();
```

### 面板显示流程
1. 检查缓存中是否有面板
2. 如果有，直接返回缓存面板
3. 如果没有，从Resources/UI/加载面板预制体
4. 实例化面板并添加到缓存
5. 调用面板的Show()方法

### 面板隐藏流程
1. 查找缓存中的面板
2. 调用面板的Hide()方法
3. 如果启用淡出，等待动画完成
4. 销毁面板并从缓存移除

## 面板预制体设置

### 预制体结构
```
Canvas
├── GameSencePanel (预制体)
│   ├── HealthBar
│   ├── StaminaBar
│   └── MiniMap
└── GameOverPanel (预制体)
    ├── GameOverText
    ├── ScoreText
    └── RestartButton
```

### 预制体配置
1. 创建Panel预制体
2. 添加CanvasGroup组件
3. 挂载继承UIPanel的脚本
4. 放置到Resources/UI目录下

## 集成示例

### 与事件系统集成
```csharp
public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        EventCenter.Instance.AddEventListener(E_EventType.GameOver, ShowGameOverPanel);
    }

    void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.GameOver, ShowGameOverPanel);
    }

    void ShowGameOverPanel()
    {
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }
}
```

### 与玩家系统集成
```csharp
public class PlayerController : MonoBehaviour
{
    void Update()
    {
        // 更新体力UI
        var panel = UIManager.Instance.GetPanel<GameSencePanel>();
        if (panel != null)
        {
            panel.ChangeEnduranceValue(maxEndurance, nowEndurance);
        }
    }
}
```

## 面板动画效果

### 启用淡入淡出
在UIPanel Inspector中勾选"启用淡入淡出"选项。

### 淡入效果
```csharp
public virtual void Show()
{
    ResetPanel();
    isShow = true;

    if (enableFade)
    {
        canvasGroup.alpha = 0;  // 从透明开始淡入
    }
    else
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }
}
```

### 淡出效果
```csharp
protected virtual void Update()
{
    if (!enableFade) return;

    if (isShow && canvasGroup.alpha != 1)
    {
        canvasGroup.alpha += alphaSpeed * Time.deltaTime;
        if (canvasGroup.alpha > 1)
            canvasGroup.alpha = 1;
    }
    else if (!isShow && canvasGroup.alpha != 0)
    {
        canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
        if (canvasGroup.alpha < 0)
        {
            canvasGroup.alpha = 0;
            hideCallBack?.Invoke();
        }
    }
}
```

## 注意事项
1. 面板预制体需要放置到Resources/UI目录下
2. 面板需要继承UIPanel类
3. 面板需要CanvasGroup组件用于淡入淡出
4. 面板名称需要与预制体名称一致
5. 避免在面板中直接引用其他面板
6. 使用UI管理工具可以快速创建面板

## 扩展功能

### 面板层级管理
```csharp
public class UILayerManager : MonoBehaviour
{
    public enum UILayer
    {
        Background,
        Normal,
        Popup,
        Top
    }

    public void SetPanelLayer(UIPanel panel, UILayer layer)
    {
        // 设置面板层级
    }
}
```

### 面板动画
```csharp
public class AnimatedPanel : UIPanel
{
    [SerializeField] private Animator animator;

    public override void Show()
    {
        base.Show();
        animator.Play("Show");
    }

    public override void Hide()
    {
        animator.Play("Hide");
        base.Hide();
    }
}
```

## 最佳实践

### 面板设计原则
1. 每个面板负责单一功能
2. 面板之间通过事件系统通信
3. 避免面板之间的直接引用
4. 使用预制体模板提高开发效率

### 性能优化
1. 合理使用面板缓存
2. 避免频繁创建销毁面板
3. 使用对象池管理面板
4. 优化面板资源加载

## 常见问题

### 面板为空
```csharp
// 确保面板已创建
if (UIManager.Instance.GetPanel<GameSencePanel>() != null)
{
    // 面板存在
}

// 或使用HasPanel检查
if (UIManager.Instance.HasPanel<GameSencePanel>())
{
    // 面板存在
}
```

### 面板重复创建
```csharp
// 使用缓存避免重复创建
if (panelDic.ContainsKey(panelName))
{
    return (T)panelDic[panelName];
}
```

### 面板销毁异常
```csharp
// 确保在淡出完成后销毁
panel.Hide(() =>
{
    GameObject.Destroy(panel.gameObject);
    panelDic.Remove(panelName);
});
```
