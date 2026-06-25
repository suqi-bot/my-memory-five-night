# 场景物品系统 (Scene Item System)

## 概述
场景物品系统用于管理游戏场景中的可交互物品，提供统一的物品交互接口。系统支持物品的交互检测、交互执行和状态管理。

## 系统架构

### 核心组件
1. **ItemInterface** - 物品交互接口
2. **Door** - 门物品实现示例
3. **FaceCheck** - 面前物品检测

## 快速开始

### 1. 创建可交互物品
```csharp
// 实现ItemInterface接口
public class Door : MonoBehaviour, ItemInterface
{
    public void UseItem()
    {
        // 交互逻辑
    }
}
```

### 2. 物品检测
```csharp
// 使用FaceCheck检测面前的物品
public class FaceCheck : MonoBehaviour
{
    public ItemInterface item;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemInterface>() != null)
        {
            item = other.GetComponent<ItemInterface>();
        }
    }
}
```

### 3. 执行交互
```csharp
// 在玩家控制器中执行交互
if (Input.GetButtonDown("Interaction"))
{
    CheckFaceRay();
}
```

## 使用示例

### 门物品
```csharp
public class Door : MonoBehaviour, ItemInterface
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip openClip;
    [SerializeField] private AnimationClip closeClip;
    private bool isOpen;
    private bool isPlaying;
    
    public void UseItem()
    {
        if (isPlaying) return;
        
        isOpen = !isOpen;
        AnimationClip clip = isOpen ? openClip : closeClip;
        animator.Play(clip.name);
        isPlaying = true;
    }
}
```

### 开关物品
```csharp
public class LightSwitch : MonoBehaviour, ItemInterface
{
    [SerializeField] private Light light;
    private bool isOn;
    
    public void UseItem()
    {
        isOn = !isOn;
        light.enabled = isOn;
    }
}
```

### 拾取物品
```csharp
public class PickupItem : MonoBehaviour, ItemInterface
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    
    public void UseItem()
    {
        // 添加到背包
        PlayerBagItem.Instance.AddItem(itemName, icon);
        
        // 销毁物品
        Destroy(gameObject);
    }
}
```

## 文件结构
```
Assets/Script/SceneItem/
├── ItemInterface.cs    # 物品交互接口
└── Door.cs             # 门物品实现
```

## 交互检测系统

### 射线检测
```csharp
public RaycastHit CheckFaceRay()
{
    Ray ray = new Ray(transform.position, Camera.main.transform.forward);
    RaycastHit hitInfo = new RaycastHit();
    
    if (Physics.Raycast(ray, out hitInfo))
    {
        // 检测到物品
        hitInfo.collider.GetComponentInParent<ItemInterface>()?.UseItem();
    }
    
    return hitInfo;
}
```

### 触发器检测
```csharp
private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
        return;
    
    if (other.GetComponent<ItemInterface>() != null)
    {
        item = other.GetComponent<ItemInterface>();
    }
}
```

## 物品状态管理

### 状态定义
```csharp
public enum ItemState
{
    Idle,       // 空闲状态
    Active,     // 激活状态
    Broken,     // 损坏状态
    Locked      // 锁定状态
}
```

### 状态切换
```csharp
public class InteractiveItem : MonoBehaviour, ItemInterface
{
    private ItemState currentState = ItemState.Idle;
    
    public void UseItem()
    {
        switch (currentState)
        {
            case ItemState.Idle:
                Activate();
                break;
            case ItemState.Active:
                Deactivate();
                break;
            case ItemState.Locked:
                ShowLockedMessage();
                break;
        }
    }
}
```

## 集成示例

### 与玩家系统集成
```csharp
public class PlayerController : MonoBehaviour
{
    public RaycastHit CheckFaceRay()
    {
        Ray ray = new Ray(transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        
        if (Physics.Raycast(ray, out hitInfo))
        {
            // 调用物品交互
            hitInfo.collider.GetComponentInParent<ItemInterface>()?.UseItem();
        }
        
        return hitInfo;
    }
}
```

### 与UI系统集成
```csharp
public class InteractionUI : MonoBehaviour
{
    public FaceCheck faceCheck;
    
    private void Update()
    {
        if (faceCheck.item != null)
        {
            // 显示交互提示
            ShowInteractionHint("按E键交互");
        }
        else
        {
            // 隐藏交互提示
            HideInteractionHint();
        }
    }
}
```

## 注意事项
1. 物品需要有Collider组件用于检测
2. 物品需要实现ItemInterface接口
3. 交互距离基于射线检测距离
4. 物品状态需要自行管理
5. 交互反馈需要在UseItem方法中实现

## 扩展功能

### 自定义交互接口
```csharp
public interface IInteractable
{
    void OnInteract(PlayerController player);
    string GetInteractionText();
    bool CanInteract(PlayerController player);
}
```

### 物品高亮
```csharp
public class ItemHighlight : MonoBehaviour
{
    public void OnFocus()
    {
        // 高亮物品
    }
    
    public void OnLoseFocus()
    {
        // 取消高亮
    }
}
```

## 最佳实践

### 物品设计原则
1. 每个物品独立管理自己的状态
2. 交互逻辑封装在物品内部
3. 使用事件系统通知其他模块
4. 提供清晰的交互反馈

### 性能优化
1. 使用对象池管理频繁创建的物品
2. 避免在Update中频繁检测物品
3. 使用Layer优化检测范围
4. 合理设置检测距离