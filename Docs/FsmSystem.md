# 有限状态机系统 (Finite State Machine System)

## 概述
有限状态机系统提供了一个通用的状态管理框架，用于管理游戏对象的状态切换和状态行为。系统支持状态的定义、切换、更新和状态数据共享。

## 系统架构

### 核心组件
1. **Fsm** - 有限状态机类
2. **IState** - 状态接口
3. **BlackBoard** - 黑板数据类

## 快速开始

### 1. 定义状态
```csharp
public class IdleState : IState
{
    private Fsm fsm;
    
    public IdleState(Fsm fsm)
    {
        this.fsm = fsm;
    }
    
    public void OnEnter()
    {
        // 进入空闲状态
        Debug.Log("进入空闲状态");
    }
    
    public void OnUpdate()
    {
        // 空闲状态更新
        if (Input.GetKeyDown(KeyCode.W))
        {
            fsm.SwitchState(PlayerState.Walking);
        }
    }
    
    public void OnExit()
    {
        // 退出空闲状态
        Debug.Log("退出空闲状态");
    }
}
```

### 2. 创建状态机
```csharp
// 创建黑板数据
BlackBoard blackBoard = new BlackBoard();

// 创建状态机
Fsm fsm = new Fsm(blackBoard);

// 添加状态
fsm.AddState(PlayerState.Idle, new IdleState(fsm));
fsm.AddState(PlayerState.Walking, new WalkingState(fsm));
fsm.AddState(PlayerState.Running, new RunningState(fsm));
```

### 3. 更新状态机
```csharp
// 在Update中调用
private void Update()
{
    fsm.Update();
}
```

## 使用示例

### 玩家状态机
```csharp
public enum PlayerState
{
    Idle,
    Walking,
    Running,
    Jumping,
    Dead
}

public class PlayerController : MonoBehaviour
{
    private Fsm fsm;
    
    private void Start()
    {
        // 创建黑板
        BlackBoard blackBoard = new BlackBoard();
        
        // 创建状态机
        fsm = new Fsm(blackBoard);
        
        // 添加状态
        fsm.AddState(PlayerState.Idle, new IdleState(fsm));
        fsm.AddState(PlayerState.Walking, new WalkingState(fsm));
        fsm.AddState(PlayerState.Running, new RunningState(fsm));
        fsm.AddState(PlayerState.Jumping, new JumpingState(fsm));
        fsm.AddState(PlayerState.Dead, new DeadState(fsm));
        
        // 初始状态
        fsm.SwitchState(PlayerState.Idle);
    }
    
    private void Update()
    {
        fsm.Update();
    }
}
```

### 敌人AI状态机
```csharp
public enum EnemyState
{
    Patrol,
    Chase,
    Attack,
    Dead
}

public class EnemyAI : MonoBehaviour
{
    private Fsm fsm;
    
    private void Start()
    {
        BlackBoard blackBoard = new BlackBoard();
        fsm = new Fsm(blackBoard);
        
        fsm.AddState(EnemyState.Patrol, new PatrolState(fsm));
        fsm.AddState(EnemyState.Chase, new ChaseState(fsm));
        fsm.AddState(EnemyState.Attack, new AttackState(fsm));
        fsm.AddState(EnemyState.Dead, new DeadState(fsm));
        
        fsm.SwitchState(EnemyState.Patrol);
    }
}
```

## 文件结构
```
Assets/Script/
├── Fsm.cs      # 有限状态机类
└── IState.cs   # 状态接口
```

## 状态接口详解

### IState接口
```csharp
public interface IState
{
    void OnEnter();   // 进入状态
    void OnUpdate();  // 状态更新
    void OnExit();    // 退出状态
}
```

### 状态实现示例
```csharp
public class WalkingState : IState
{
    private Fsm fsm;
    private float speed = 5f;
    
    public WalkingState(Fsm fsm)
    {
        this.fsm = fsm;
    }
    
    public void OnEnter()
    {
        // 进入行走状态
        Debug.Log("开始行走");
    }
    
    public void OnUpdate()
    {
        // 行走逻辑
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // 移动逻辑
        
        // 状态切换
        if (horizontal == 0 && vertical == 0)
        {
            fsm.SwitchState(PlayerState.Idle);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            fsm.SwitchState(PlayerState.Running);
        }
    }
    
    public void OnExit()
    {
        // 退出行走状态
        Debug.Log("停止行走");
    }
}
```

## 黑板系统

### BlackBoard类
```csharp
[Serializable]
public class BlackBoard
{
    // 状态间共享的数据
    public float health;
    public float stamina;
    public Vector3 targetPosition;
    public bool isGrounded;
}
```

### 使用黑板
```csharp
public class IdleState : IState
{
    private Fsm fsm;
    
    public IdleState(Fsm fsm)
    {
        this.fsm = fsm;
    }
    
    public void OnEnter()
    {
        // 访问黑板数据
        Debug.Log("当前生命值: " + fsm.blackBoard.health);
    }
    
    public void OnUpdate()
    {
        // 修改黑板数据
        fsm.blackBoard.stamina += Time.deltaTime;
        
        // 状态切换逻辑
        if (fsm.blackBoard.health <= 0)
        {
            fsm.SwitchState(PlayerState.Dead);
        }
    }
    
    public void OnExit()
    {
        // 退出状态
    }
}
```

## 状态切换

### 切换状态
```csharp
// 切换到指定状态
fsm.SwitchState(PlayerState.Walking);
```

### 切换流程
1. 调用当前状态的OnExit方法
2. 更新当前状态引用
3. 调用新状态的OnEnter方法

## 集成示例

### 与玩家系统集成
```csharp
public class PlayerController : MonoBehaviour
{
    private Fsm fsm;
    
    private void Start()
    {
        BlackBoard blackBoard = new BlackBoard();
        fsm = new Fsm(blackBoard);
        
        // 添加状态
        fsm.AddState(PlayerState.Idle, new IdleState(fsm));
        fsm.AddState(PlayerState.Walking, new WalkingState(fsm));
        
        // 初始状态
        fsm.SwitchState(PlayerState.Idle);
    }
    
    private void Update()
    {
        // 更新状态机
        fsm.Update();
        
        // 更新黑板数据
        fsm.blackBoard.isGrounded = CheckGrounded();
        fsm.blackBoard.health = currentHealth;
    }
}
```

### 与动画系统集成
```csharp
public class PlayerState : IState
{
    protected Animator animator;
    
    public void OnEnter()
    {
        // 播放动画
        animator.Play("Idle");
    }
}
```

## 注意事项
1. 状态机需要手动调用Update方法
2. 状态切换会立即执行OnExit和OnEnter
3. 黑板数据需要手动同步
4. 避免在状态中直接修改状态机
5. 状态应该只关注自己的逻辑

## 扩展功能

### 层次状态机
```csharp
public class HierarchicalFsm
{
    private Dictionary<string, Fsm> subFsms;
    private Fsm currentSubFsm;
}
```

### 并行状态机
```csharp
public class ParallelFsm
{
    private List<Fsm> parallelFsms;
    
    public void Update()
    {
        foreach (var fsm in parallelFsms)
        {
            fsm.Update();
        }
    }
}
```

## 最佳实践

### 状态设计原则
1. 每个状态应该单一职责
2. 状态之间应该低耦合
3. 使用黑板共享数据
4. 避免状态循环依赖

### 性能优化
1. 避免在状态中频繁创建对象
2. 使用对象池管理状态对象
3. 合理设计状态切换条件
4. 避免状态机过度复杂

## 常见问题

### 状态切换失败
```csharp
// 确保状态已添加
if (stateDic.ContainsKey(stateType))
{
    fsm.SwitchState(stateType);
}
```

### 状态更新异常
```csharp
// 确保在Update中调用
private void Update()
{
    fsm.Update();
}
```

### 黑板数据不同步
```csharp
// 在Update中同步数据
private void Update()
{
    fsm.blackBoard.health = currentHealth;
    fsm.Update();
}
```