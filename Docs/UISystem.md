# UI系统 (UI System)

## 概述
UI系统负责管理游戏中的用户界面，包括面板的显示、隐藏、切换和管理。系统支持面板的淡入淡出效果、面板缓存和UI层级管理。

## 系统架构

### 核心组件
1. **UIPanel** - 面板基类
2. **UIManager** - UI管理器（单例模式）
3. **EventManager** - 事件管理器
4. **GameSencePanel** - 游戏场景面板示例

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

## 使用示例

### 创建新面板
```csharp
public class MainMenuPanel : UIPanel
{
    public Button startButton;
    public Button exitButton;

    public override void Init(object data = null)
    {
        base.Init(data);
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public override void Show()
    {
        base.Show();
        // 显示时的逻辑
    }

    public override void Hide()
    {
        base.Hide();
        // 隐藏时的逻辑
    }

    private void OnStartButtonClicked()
    {
        // 开始游戏
        UIManager.Instance.HidePanel<MainMenuPanel>();
        UIManager.Instance.ShowPanel<GameSencePanel>();
    }

    private void OnExitButtonClicked()
    {
        // 退出游戏
        Application.Quit();
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

### 使用事件系统控制面板
```csharp
// 通过事件显示面板
EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryShowUIPanel, UIPanelType.通关);

// 通过事件隐藏面板
EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryHideUIPanel, UIPanelType.通关);

// 通过事件切换面板
EventManager.Ins.EventTrigger<UIPanelType, bool>(EventType.SwitchUIPanel, UIPanelType.通关, true);
```

## 文件结构
```
Assets/Script/UI/
└── GameSencePanel.cs    # 游戏场景面板

Assets/Script/ZUI/
├── UIPanel.cs           # 面板基类
├── UIManager.cs         # UI管理器
├── PanelBtn.cs          # 面板按钮
├── PopupBase.cs         # 弹窗基类
└── ...

Assets/Script/EventCenter/
├── EventManager.cs      # 事件管理器
├── EventCenter.cs       # 事件中心
└── EventType.cs         # 事件类型
```

## 面板管理

### 面板缓存
```csharp
// UIManager自动缓存已创建的面板
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

## 动画效果

### 启用淡入淡出
在UIPanel Inspector中勾选"启用淡入淡出"选项，或在代码中设置：
```csharp
// 在Inspector中设置enableFade = true
// 或通过代码控制
```

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

## 面板预制体设置

### 预制体结构
```
Canvas
├── MainMenuPanel (预制体)
│   ├── Background
│   ├── Title
│   ├── StartButton
│   └── ExitButton
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

## 输入处理

### 按钮事件绑定
```csharp
public class MainMenuPanel : UIPanel
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    public override void Init(object data = null)
    {
        base.Init(data);
        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        // 开始游戏
    }

    private void OnExitButtonClicked()
    {
        // 退出游戏
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
