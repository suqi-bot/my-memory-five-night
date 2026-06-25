# 面前物品检测系统 (Face Check System)

## 概述
面前物品检测系统用于检测玩家面前的可交互物品，为物品交互提供检测支持。系统使用触发器检测机制，自动识别玩家面前的物品。

## 系统架构

### 核心组件
1. **FaceCheck** - 面前物品检测器

## 快速开始

### 1. 添加检测器
```csharp
// 在玩家身上添加检测器
public class PlayerController : MonoBehaviour
{
    public FaceCheck faceCheck;
    
    private void Start()
    {
        faceCheck = GetComponentInChildren<FaceCheck>();
    }
}
```

### 2. 检测物品
```csharp
// 检测面前的物品
if (faceCheck.item != null)
{
    // 检测到物品
    faceCheck.item.UseItem();
}
```

### 3. 物品交互
```csharp
// 在玩家控制器中执行交互
if (Input.GetButtonDown("Interaction"))
{
    if (faceCheck.item != null)
    {
        faceCheck.item.UseItem();
    }
}
```

## 使用示例

### 检测器配置
```csharp
public class FaceCheck : MonoBehaviour
{
    public ItemInterface item;
    
    private void Start()
    {
        item = null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            return;
        
        if (other.GetComponent<ItemInterface>() != null)
        {
            item = other.GetComponent<ItemInterface>();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ItemInterface>() != null)
        {
            item = null;
        }
    }
}
```

### 物品交互
```csharp
public class PlayerController : MonoBehaviour
{
    public FaceCheck faceCheck;
    
    private void Update()
    {
        if (Input.GetButtonDown("Interaction"))
        {
            if (faceCheck.item != null)
            {
                faceCheck.item.UseItem();
            }
        }
    }
}
```

## 文件结构
```
Assets/Script/
└── FaceCheck.cs    # 面前物品检测器
```

## 检测机制

### 触发器检测
```csharp
private void OnTriggerEnter(Collider other)
{
    // 检测进入触发器的物体
    if (other.GetComponent<ItemInterface>() != null)
    {
        item = other.GetComponent<ItemInterface>();
    }
}

private void OnTriggerExit(Collider other)
{
    // 检测离开触发器的物体
    if (other.GetComponent<ItemInterface>() != null)
    {
        item = null;
    }
}
```

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

## 集成示例

### 与玩家系统集成
```csharp
public class PlayerController : MonoBehaviour
{
    public FaceCheck faceCheck;
    
    private void Update()
    {
        // 检测面前物品
        if (faceCheck.item != null)
        {
            // 显示交互提示
            ShowInteractionHint();
        }
        
        // 执行交互
        if (Input.GetButtonDown("Interaction"))
        {
            if (faceCheck.item != null)
            {
                faceCheck.item.UseItem();
            }
        }
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
1. 检测器需要有Collider组件设置为触发器
2. 物品需要有Collider组件
3. 物品需要实现ItemInterface接口
4. 检测范围基于触发器大小
5. 避免检测器与其他触发器冲突

## 扩展功能

### 多物品检测
```csharp
public class MultiFaceCheck : MonoBehaviour
{
    public List<ItemInterface> items = new List<ItemInterface>();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemInterface>() != null)
        {
            items.Add(other.GetComponent<ItemInterface>());
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ItemInterface>() != null)
        {
            items.Remove(other.GetComponent<ItemInterface>());
        }
    }
    
    public ItemInterface GetNearestItem()
    {
        // 获取最近的物品
        return items.FirstOrDefault();
    }
}
```

### 检测范围可视化
```csharp
public class FaceCheckVisual : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
```

## 最佳实践

### 检测器设计原则
1. 检测器应该放在玩家前方
2. 检测范围应该适中
3. 避免检测器过于敏感
4. 提供清晰的交互反馈

### 性能优化
1. 使用Layer优化检测范围
2. 避免检测器过大
3. 合理设置触发器大小
4. 使用对象池管理检测器