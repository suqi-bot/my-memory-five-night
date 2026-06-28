# ZUI系统 (ZUI System)

## 概述
ZUI系统是一套从其他项目导入的UI组件库，提供了丰富的UI控件和导航功能。系统包含按钮组件、面板管理、进度条、选择效果、导航系统等，与项目的EventCenter系统集成实现事件通信。

## 系统架构

### 核心组件
1. **UIPanel** - 功能面板基类（事件驱动）
2. **UIManager** - UI管理器（单例模式）
3. **PanelBtn** - 面板按钮组件
4. **PopupBase** - 弹窗基类
5. **ProgressBar** - 进度条组件
6. **UISelectableEffectBase** - 选择效果基类

### 导航组件
7. **MUINav_Controller** - 导航控制器
8. **MUINav_Grid** - 导航网格
9. **MUINav_Item** - 导航项

### 工具组件
10. **SafeAreaAdapter** - 安全区域适配器
11. **LinkButton** - 链接按钮
12. **SendEventBtn** - 事件发送按钮

## 快速开始

### 1. 使用UIManager显示面板
```csharp
// 显示面板
GameSencePanel panel = UIManager.Instance.ShowPanel<GameSencePanel>();

// 隐藏面板（带淡出效果）
UIManager.Instance.HidePanel<GameSencePanel>();

// 隐藏面板（无淡出效果）
UIManager.Instance.HidePanel<GameSencePanel>(false);

// 获取已显示的面板
GameSencePanel panel = UIManager.Instance.GetPanel<GameSencePanel>();
```

### 2. 使用UIPanel事件系统
```csharp
// 显示指定面板
EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryShowUIPanel, UIPanelType.通关);

// 隐藏指定面板
EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryHideUIPanel, UIPanelType.通关);

// 切换面板显示状态
EventManager.Ins.EventTrigger<UIPanelType, bool>(EventType.SwitchUIPanel, UIPanelType.通关, true);
```

### 3. 创建功能面板
```csharp
public class MyPanel : UIPanel
{
    public override void Init(object data)
    {
        // 初始化面板
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
}
```

## 使用示例

### PanelBtn面板按钮
```csharp
// PanelBtn用于触发面板显示/隐藏
// 在Inspector中设置：
// - panelType: 目标面板类型
// - isIndependent: 是否独立于UIManager管理
// - panel: 直接引用UIPanel（优先级最高）

// 点击按钮时自动触发事件
public void OnClick()
{
    if (panel != null)
        panel.Switch();
    else if (isIndependent)
        EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryShowUIPanel, panelType);
    else
        EventManager.Ins.EventTrigger<UIPanelType, object>(EventType.ShowPanel, panelType, null);
}
```

### ProgressBar进度条
```csharp
public class MyController : MonoBehaviour
{
    public ProgressBar progressBar;

    void Start()
    {
        // 开始5秒倒计时
        progressBar.StartCountdown(5f);
        progressBar.onComplete += OnCountdownComplete;
    }

    void OnCountdownComplete()
    {
        Debug.Log("倒计时完成");
    }

    // 手动设置进度 (0-1)
    void SetProgress(float value)
    {
        progressBar.SetProgress(value);
    }
}
```

### UISelectableEffect选择效果
```csharp
// UISelectableEffect_Button - 按钮选择效果
// 自动处理选中/取消选中的视觉反馈

// UISelectableEffect_Toggle - Toggle选择效果
// 与Toggle组件集成

// 在Inspector中配置：
// - imageEffect: 启用图片效果
// - textEffect: 启用文本效果
// - selectedScale: 选中时的缩放
// - showObjs/hideObjs: 选中时显示/隐藏的对象
```

### MUINav导航系统
```csharp
public class MyMenu : MonoBehaviour
{
    public MUINav_Controller navController;

    void Start()
    {
        // 初始化导航
        navController.Init();
    }

    // 设置起始项
    void SetStartItem(int index)
    {
        navController.SetStartItem(index);
    }

    // 启用/禁用导航
    void SetNavigationActive(bool active)
    {
        navController.SetActive(active);
    }
}
```

### SendEventBtn事件按钮
```csharp
// SendEventBtn用于发送自定义事件
// 在Inspector中设置eventType

// 点击按钮时自动触发对应事件
public virtual void Awake()
{
    button.onClick.AddListener(() =>
    {
        EventManager.Ins.EventTrigger(eventType);
    });
}
```

### SafeAreaAdapter安全区域
```csharp
// SafeAreaAdapter用于适配异形屏
// 自动根据Screen.safeArea调整RectTransform

// 挂载到需要适配安全区域的UI元素上
// 在Start时自动调整
```

## 文件结构
```
Assets/Script/ZUI/
├── UIManager.cs                    # UI管理器
├── UIPanel.cs                      # 功能面板基类
├── PanelBtn.cs                     # 面板按钮
├── PopupBase.cs                    # 弹窗基类
├── ProgressBar.cs                  # 进度条
├── LinkButton.cs                   # 链接按钮
├── SendEventBtn.cs                 # 事件发送按钮
├── JoystickBtn.cs                  # 手柄按钮（预留）
├── UI_LRBtnAndText.cs              # 左右按钮文本组件
├── UISelectableEffectBase.cs       # 选择效果基类
├── UISelectableEffect_Button.cs    # 按钮选择效果
├── UISelectableEffect_Toggle.cs    # Toggle选择效果
├── UINavigation/
│   ├── MUINav_Controller.cs        # 导航控制器
│   ├── MUINav_Grid.cs              # 导航网格
│   ├── MUINav_Item.cs              # 导航项
│   ├── ScrollViewFocus.cs          # 滚动视图聚焦
│   └── VerticalButtonNavigation.cs # 垂直按钮导航
└── Tools/
    └── Android/
        └── SafeAreaAdapter.cs      # 安全区域适配器
```

## 面板类型说明

### UIPanelType枚举
```csharp
public enum UIPanelType
{
    其它,       // 默认类型
    失败,       // 失败面板
    通关,       // 通关面板
    快速选关,   // 快速选关面板
    章节,       // 章节面板
    教学,       // 教学面板
    操作选择,   // 操作选择面板
    故事线,     // 故事线面板
    章节结束,   // 章节结束面板
    QTE,        // QTE面板
    帮助,       // 帮助面板
    故事线_章节, // 故事线章节面板
    成就,       // 成就面板
    QTE_点击,   // QTE点击面板
    QTE_滑动,   // QTE滑动面板
    QTE_稳定控制, // QTE稳定控制面板
    档案,       // 档案面板
    循环节点_UI面板, // 循环节点面板
    档案_选择头像   // 档案选择头像面板
}
```

## 事件类型说明

### EventType枚举
```csharp
public enum EventType
{
    TryShowUIPanel,   // 尝试显示UI面板
    TryHideUIPanel,   // 尝试隐藏UI面板
    SwitchUIPanel,    // 切换UI面板
    UITryDO,          // UI尝试执行
    GameUIShow,       // 游戏UI显示
    GameUIHide,       // 游戏UI隐藏
    HidePanel,        // 隐藏面板
    RemovePanel,      // 移除面板
    ShowPanel         // 显示面板
}
```

## 工作原理

### UIManager面板管理流程
1. **显示面板**
   - 检查面板缓存字典
   - 如果缓存中存在，直接返回
   - 如果不存在，从Resources/UI/加载预制体
   - 实例化面板并添加到缓存
   - 调用panel.ShowMe()显示面板

2. **隐藏面板**
   - 从缓存中查找面板
   - 如果isFade=true，调用HideMe()并等待淡出
   - 淡出完成后销毁面板并从缓存移除
   - 如果isFade=false，直接销毁

3. **获取面板**
   - 从缓存中查找面板
   - 返回面板实例或null

### UIPanel事件驱动机制
1. **OnEnable时注册事件**
   - 监听TryShowUIPanel事件
   - 监听TryHideUIPanel事件
   - 监听SwitchUIPanel事件
   - 监听UITryDO事件

2. **OnDisable时移除事件**
   - 移除所有注册的事件监听

3. **事件处理**
   - TryShow: 检查panelType匹配后显示
   - TryHide: 检查panelType匹配后隐藏
   - Switch: 根据参数切换显示状态

### 导航系统工作流程
1. **初始化**
   - MUINav_Controller.Init()初始化导航
   - MUINav_Grid.InitGrid()初始化网格
   - 计算所有导航项的邻居关系

2. **导航操作**
   - 检测输入方向（上下左右）
   - 查找当前选中项的对应方向邻居
   - 取消当前选中，选中新项
   - 更新视觉效果

3. **点击操作**
   - 检测确认键输入
   - 触发当前选中项的OnClick事件
   - 处理取消点击逻辑

## 集成示例

### 与EventCenter系统集成
```csharp
// ZUI系统使用EventManager，与EventCenter并行工作
// EventManager专门处理UI相关事件
// EventCenter处理游戏逻辑事件

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        // 监听UI事件
        EventManager.Ins.AddEventListener<UIPanelType>(EventType.HidePanel, OnPanelHidden);

        // 监听游戏事件
        EventCenter.Instance.AddEventListener(E_EventType.GameOver, OnGameOver);
    }

    void OnPanelHidden(UIPanelType panelType)
    {
        Debug.Log($"面板已隐藏: {panelType}");
    }

    void OnGameOver()
    {
        // 显示游戏结束面板
        EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryShowUIPanel, UIPanelType.失败);
    }
}
```

### 完整UI流程示例
```csharp
public class UIManagerSetup : MonoBehaviour
{
    void Start()
    {
        // 初始化UIManager（自动创建Canvas）
        var uiManager = UIManager.Instance;

        // 显示主菜单
        uiManager.ShowPanel<MainMenuPanel>();
    }
}

public class MainMenuPanel : UIPanel
{
    public Button startBtn;
    public Button settingsBtn;

    public override void Init(object data)
    {
        startBtn.onClick.AddListener(OnStartClicked);
        settingsBtn.onClick.AddListener(OnSettingsClicked);
    }

    void OnStartClicked()
    {
        // 隐藏主菜单
        EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryHideUIPanel, UIPanelType.其它);
        // 显示游戏UI
        EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryShowUIPanel, UIPanelType.教学);
    }

    void OnSettingsClicked()
    {
        // 显示设置面板
        EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryShowUIPanel, UIPanelType.帮助);
    }
}
```

### 导航系统集成
```csharp
public class MainMenuNavigation : MonoBehaviour
{
    public MUINav_Controller navController;
    public MUINav_Grid menuGrid;

    void Start()
    {
        // 初始化导航网格
        menuGrid.InitGrid();
        // 设置起始选中项
        navController.SetStartItem(0);
        // 启用导航
        navController.SetActive(true);
    }

    void Update()
    {
        // 导航控制器自动处理输入
        // 上下左右移动选择
        // 确认键点击选中项
    }
}
```

## UI管理工具

### 打开工具
通过菜单 `Tools > UI Manager Tool` 打开UI管理工具窗口。

### 功能说明

#### 1. 创建面板 (Create Tab)
- **面板名称**: 输入新面板的名称
- **面板描述**: 为面板添加描述说明
- **面板类型**: 选择UIPanel类型
- **创建预制体**: 自动创建预制体文件
- **添加到Resources**: 将预制体创建到Resources/UI目录
- **启用淡入淡出**: 面板显示隐藏时的淡入淡出效果

#### 2. 面板列表 (List Tab)
- 查看所有已创建的UI面板脚本
- 点击"脚本"按钮打开脚本编辑
- 点击"预制体"按钮选中预制体文件
- 支持删除面板（同时删除脚本和预制体）

#### 3. 设置 (Settings Tab)
- 自定义脚本保存路径
- 自定义预制体保存路径
- 创建和管理模板文件夹

### 模板自定义
在 `Assets/Editor/UI/Templates/` 目录下可以自定义模板：
- `UIPanelTemplate.txt` - UIPanel模板

可用变量：
- `{PANEL_NAME}` - 面板名称
- `{PANEL_DESC}` - 面板描述
- `{PANEL_TYPE}` - 面板类型

## 注意事项
1. UIPanelType枚举可以根据项目需求扩展
2. 面板预制体需要放置到Resources/UI/目录下
3. 面板需要挂载CanvasGroup组件
4. 导航系统需要EventSystem支持
5. 安全区域适配器主要用于移动端异形屏
6. EventManager与EventCenter是并行的两套事件系统
7. 使用UI管理工具可以快速创建和管理UI面板

## 扩展功能

### 自定义面板类型
```csharp
// 扩展UIPanelType枚举
public enum UIPanelType
{
    其它,
    失败,
    通关,
    // 添加新类型
    商店,
    背包,
    设置,
    聊天
}
```

### 自定义选择效果
```csharp
public class MySelectableEffect : UISelectableEffectBase
{
    public override void OnSelect()
    {
        base.OnSelect();
        // 添加自定义选中效果
        // 例如：播放音效、粒子效果等
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        // 添加自定义取消选中效果
    }
}
```

### 自定义导航行为
```csharp
public class MyNavController : MUINav_Controller
{
    public override void MoveTo(MUINav_Item item)
    {
        base.MoveTo(item);
        // 添加自定义移动逻辑
        // 例如：播放移动音效、更新描述文本等
    }

    public override void TryClick(MUINav_Item item, bool update = true)
    {
        base.TryClick(item, update);
        // 添加自定义点击逻辑
    }
}
```

## 最佳实践

### 面板设计原则
1. 使用UIPanelType分类管理面板
2. 面板之间通过EventManager通信
3. 避免面板之间的直接引用
4. 合理使用面板缓存机制

### 导航系统使用
1. 合理设置导航网格的列数
2. 确保导航项的邻居关系正确
3. 处理边界情况（如循环导航）
4. 提供视觉反馈增强用户体验

### 性能优化
1. 合理使用面板缓存
2. 避免频繁创建销毁面板
3. 使用对象池管理面板实例
4. 优化面板资源加载

### 事件系统使用
1. 在OnEnable中注册事件，在OnDisable中移除
2. 避免在事件回调中执行耗时操作
3. 合理设计事件类型，避免事件泛滥
4. 及时移除事件监听避免内存泄漏
