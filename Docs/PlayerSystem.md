# 玩家系统 (Player System)

## 概述
玩家系统负责管理玩家的移动、交互、状态控制等功能。系统支持第一人称视角控制、移动、跳跃、奔跑、蹲下以及物品交互。

## 系统架构

### 核心组件
1. **PlayerController** - 玩家控制器，管理移动和交互
2. **PlayerMouse** - 鼠标控制，管理视角旋转
3. **PlayerFsmState** - 玩家状态机状态
4. **PlayerBagItem** - 玩家背包物品
5. **PlayerState** - 玩家状态目录

## 快速开始

### 1. 基本移动
```csharp
// 玩家移动由PlayerController自动处理
// 使用WASD键移动，空格键跳跃，Shift键奔跑
```

### 2. 视角控制
```csharp
// 鼠标控制由PlayerMouse组件自动处理
// 鼠标移动控制视角旋转
```

### 3. 物品交互
```csharp
// 按E键与物品交互
// 系统会自动检测前方的物品并调用交互方法
```

## 功能详解

### 移动系统
- **基本移动**: WASD键控制前后左右移动
- **奔跑**: 按住Shift键加速移动，消耗体力
- **跳跃**: 空格键跳跃，消耗体力
- **蹲下**: 按Ctrl键切换蹲下状态，降低移动速度

### 体力系统
- **最大体力**: maxEndurance
- **当前体力**: nowEndurance
- **体力恢复**: 停止运动后延迟恢复
- **体力消耗**: 奔跑和跳跃消耗体力

### 交互系统
- **交互距离**: 基于射线检测
- **交互对象**: 实现ItemInterface接口的物品
- **交互反馈**: 自动调用物品的UseItem方法

## 使用示例

### 玩家控制器配置
```csharp
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;      // 移动速度
    public float runSpeed = 8f;       // 奔跑速度
    public float jumpSpeed = 5f;      // 跳跃速度
    public float maxEndurance = 100f; // 最大体力
    public float maxColdEndurance = 2f; // 体力恢复延迟
    public float griority = 9.8f;     // 重力
}
```

### 视角控制
```csharp
public class PlayerMouse : MonoBehaviour
{
    public float mouseSensitivity = 2f; // 鼠标灵敏度
    public Transform playerBody;        // 玩家身体
}
```

### 物品交互
```csharp
public interface ItemInterface
{
    void UseItem();
}

public class Door : MonoBehaviour, ItemInterface
{
    public void UseItem()
    {
        // 开关门逻辑
    }
}
```

## 文件结构
```
Assets/Script/Player/
├── PlayerController.cs    # 玩家控制器
├── PlayerMouse.cs         # 鼠标控制
├── PlayerFsmState.cs      # 玩家状态
├── PlayerBagItem.cs       # 背包物品
└── PlayerState/           # 玩家状态目录
```

## 状态机系统

### 状态定义
```csharp
public class PlayerFsmState : IState
{
    public void OnEnter()
    {
        // 进入状态
    }
    
    public void OnUpdate()
    {
        // 状态更新
    }
    
    public void OnExit()
    {
        // 退出状态
    }
}
```

### 状态切换
```csharp
// 切换到不同状态
fsm.SwitchState(PlayerState.Idle);
fsm.SwitchState(PlayerState.Walking);
fsm.SwitchState(PlayerState.Running);
```

## 物理系统

### 地面检测
```csharp
// 使用球形检测判断是否在地面
isGround = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
```

### 重力系统
```csharp
// 应用重力
velocity.y -= griority * Time.deltaTime;
cc.Move(velocity * Time.deltaTime);
```

## 输入系统

### 默认输入映射
| 输入 | 功能 |
|------|------|
| W/A/S/D | 移动 |
| Space | 跳跃 |
| Shift | 奔跑 |
| Ctrl | 蹲下 |
| E | 交互 |
| 鼠标移动 | 视角旋转 |

### 自定义输入
```csharp
// 可以在Unity Input Manager中自定义输入映射
// 修改ProjectSettings/InputManager.asset
```

## 集成示例

### 与UI系统集成
```csharp
public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        // 更新体力UI
        UIManager.Instance.GetPanel<GameSencePanel>()
            .ChangeEnduranceValue(maxEndurance, nowEndurance);
    }
}
```

### 与事件系统集成
```csharp
public class PlayerController : MonoBehaviour
{
    private void Die()
    {
        // 触发玩家死亡事件
        EventCenter.Instance.EventTrigger(E_EventType.PlayerDeath);
    }
}
```

## 注意事项
1. 玩家控制器需要CharacterController组件
2. 需要设置groundLayer层用于地面检测
3. 鼠标控制需要锁定光标
4. 体力系统需要UI支持显示体力值
5. 交互系统需要物品实现ItemInterface接口

## 扩展功能

### 自定义状态
```csharp
// 可以添加更多玩家状态
public enum PlayerState
{
    Idle,
    Walking,
    Running,
    Jumping,
    Crouching,
    Interacting,
    Dead
}
```

### 自定义交互
```csharp
// 可以扩展交互系统
public interface IInteractable
{
    void OnInteract(PlayerController player);
    string GetInteractionText();
}
```