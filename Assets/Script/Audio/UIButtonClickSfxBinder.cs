using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI按钮点击音效自动绑定器。
/// 负责为当前已加载场景中的 Button 自动补充 UIButtonClickSfxHandler。
/// </summary>
[DisallowMultipleComponent]
public class UIButtonClickSfxBinder : MonoBehaviour
{
    #region 配置常量

    /// <summary>
    /// 动态UI按钮扫描间隔。
    /// </summary>
    private const float ScanInterval = 0.5f;

    #endregion

    #region 运行时状态

    /// <summary>
    /// 已经处理过的按钮集合，避免重复添加组件。
    /// </summary>
    private readonly HashSet<Button> boundButtons = new HashSet<Button>();

    /// <summary>
    /// 清理已销毁按钮时使用的临时缓存。
    /// </summary>
    private readonly List<Button> cleanupBuffer = new List<Button>();

    /// <summary>
    /// 下一次扫描前的倒计时。
    /// </summary>
    private float scanTimer;

    #endregion

    #region 生命周期

    private void OnEnable()
    {
        // 1. 启用时立即扫描一次，覆盖场景启动时已经存在的按钮。
        BindLoadedButtons();
    }

    private void Update()
    {
        // 1. 按间隔扫描，覆盖运行时动态创建的弹窗和面板按钮。
        scanTimer -= Time.unscaledDeltaTime;
        if (scanTimer > 0f)
        {
            return;
        }

        scanTimer = ScanInterval;
        RemoveDestroyedButtons();
        BindLoadedButtons();
    }

    #endregion

    #region 绑定逻辑

    /// <summary>
    /// 为当前已加载对象中的所有按钮绑定点击音效处理器。
    /// </summary>
    private void BindLoadedButtons()
    {
#if UNITY_2023_1_OR_NEWER
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        Button[] buttons = Object.FindObjectsOfType<Button>(true);
#endif

        // 1. 遍历当前场景里能找到的按钮，并逐个尝试补齐点击音效组件。
        for (int i = 0; i < buttons.Length; i++)
        {
            TryBindButton(buttons[i]);
        }
    }

    /// <summary>
    /// 尝试给单个按钮添加点击音效处理器。
    /// </summary>
    private void TryBindButton(Button button)
    {
        // 1. 空按钮或已经处理过的按钮不重复处理。
        if (button == null || boundButtons.Contains(button))
        {
            return;
        }

        // 2. 带有忽略标记的按钮或面板不自动播放通用点击音效。
        if (button.GetComponentInParent<UIButtonClickSfxIgnore>(true) != null)
        {
            boundButtons.Add(button);
            return;
        }

        // 3. 没有处理器时自动添加；已有处理器时直接复用。
        if (button.GetComponent<UIButtonClickSfxHandler>() == null)
        {
            button.gameObject.AddComponent<UIButtonClickSfxHandler>();
        }

        boundButtons.Add(button);
    }

    /// <summary>
    /// 移除已经被销毁的按钮引用。
    /// </summary>
    private void RemoveDestroyedButtons()
    {
        // 1. 没有缓存按钮时无需清理。
        if (boundButtons.Count == 0)
        {
            return;
        }

        cleanupBuffer.Clear();

        foreach (Button button in boundButtons)
        {
            if (button == null)
            {
                cleanupBuffer.Add(button);
            }
        }

        // 2. 将已销毁按钮从集合中移除，避免集合长期增长。
        for (int i = 0; i < cleanupBuffer.Count; i++)
        {
            boundButtons.Remove(cleanupBuffer[i]);
        }

        cleanupBuffer.Clear();
    }

    #endregion
}
