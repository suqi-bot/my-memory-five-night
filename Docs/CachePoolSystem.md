# 缓存池系统 (Cache Pool System)

## 概述
缓存池系统用于管理游戏对象的复用，避免频繁创建和销毁GameObject带来的性能开销。通过对象池技术，可以显著减少内存分配和垃圾回收的开销。

## 系统架构

### 核心组件
1. **CachePoolMgr** - 缓存池管理器（单例模式）
2. **PoolData** - 池数据类，管理单个对象池

## 快速开始

### 1. 获取对象
```csharp
// 从缓存池获取对象
CachePoolMgr.Instance.GetObj("Enemy", (obj) =>
{
    // 设置对象位置
    obj.transform.position = spawnPoint.position;
    
    // 对象已激活，可以直接使用
    obj.SetActive(true);
}, spawnPoint.position);
```

### 2. 回收对象
```csharp
// 将对象回收到缓存池
CachePoolMgr.Instance.PushObj("Enemy", enemyObject);
```

### 3. 清空缓存
```csharp
// 清空所有缓存池
CachePoolMgr.Instance.ClearCache();
```

## 使用示例

### 敌人生成系统
```csharp
public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    
    public void SpawnEnemy()
    {
        // 从缓存池获取敌人对象
        CachePoolMgr.Instance.GetObj("Enemy", (enemy) =>
        {
            // 随机选择生成点
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
            
            // 初始化敌人
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.Initialize();
        }, Vector3.zero);
    }
    
    public void RecycleEnemy(GameObject enemy)
    {
        // 回收敌人到缓存池
        CachePoolMgr.Instance.PushObj("Enemy", enemy);
    }
}
```

### 子弹系统
```csharp
public class BulletSystem : MonoBehaviour
{
    public void FireBullet(Vector3 position, Vector3 direction)
    {
        CachePoolMgr.Instance.GetObj("Bullet", (bullet) =>
        {
            bullet.transform.position = position;
            bullet.transform.forward = direction;
            
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Initialize(direction);
        }, position);
    }
}
```

## 工作原理

### 对象池结构
```
Pool (GameObject)
├── Enemy_0 (GameObject)
├── Enemy_1 (GameObject)
├── Enemy_2 (GameObject)
└── ...
```

### 获取对象流程
1. 检查缓存池中是否有可用对象
2. 如果有，直接返回并激活对象
3. 如果没有，创建新对象（当前版本需要资源管理器支持）

### 回收对象流程
1. 将对象设置为非激活状态
2. 移动到池根节点下
3. 添加到可用对象列表

## 文件结构
```
Assets/Script/CachePool/
├── CachePoolMgr.cs    # 缓存池管理器
└── PoolData.cs        # 池数据类
```

## 配置说明

### 对象命名规则
- 对象名称必须与缓存池名称一致
- 例如：缓存池名称为"Enemy"，则对象名称也应为"Enemy"

### 池管理
- 每个对象池独立管理
- 支持动态扩展
- 自动回收未使用对象

## 性能优化

### 内存管理
- 避免频繁创建和销毁对象
- 减少垃圾回收压力
- 提高游戏运行效率

### 使用建议
1. 对于频繁创建销毁的对象使用缓存池
2. 合理设置池的初始大小
3. 及时回收不再使用的对象
4. 避免同时创建大量对象

## 注意事项
1. 当前版本的资源加载功能需要资源管理器支持
2. 对象名称必须与缓存池名称匹配
3. 回收对象前确保对象不再被使用
4. 清空缓存池会释放所有缓存的对象

## 扩展功能

### 自定义池管理
```csharp
// 可以扩展CachePoolMgr添加以下功能：
// 1. 预加载功能
// 2. 池大小限制
// 3. 自动回收时间
// 4. 对象重置回调
```

## 集成示例

### 与事件系统集成
```csharp
public class EnemyDeathHandler : MonoBehaviour
{
    private void OnEnable()
    {
        EventCenter.Instance.AddEventListener<E_EventType>(E_EventType.EnemyDeath, OnEnemyDeath);
    }
    
    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener<E_EventType>(E_EventType.EnemyDeath, OnEnemyDeath);
    }
    
    private void OnEnemyDeath(GameObject enemy)
    {
        // 回收敌人对象
        CachePoolMgr.Instance.PushObj("Enemy", enemy);
    }
}
```