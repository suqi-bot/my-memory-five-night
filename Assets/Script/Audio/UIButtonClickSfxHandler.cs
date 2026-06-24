using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 单个按钮的通用点击音效处理器。
/// 使用 PointerDown 触发，确保切场景按钮也能先播放音效。
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class UIButtonClickSfxHandler : MonoBehaviour, IPointerDownHandler
{
    #region 配置常量

    /// <summary>
    /// 通用UI按钮点击音效ID。
    /// </summary>
    private const string ButtonClickAudioId = "sfx_点击按钮";

    #endregion

    #region 组件引用

    /// <summary>
    /// 当前物体上的按钮组件。
    /// </summary>
    private Button button;

    #endregion

    #region 生命周期

    private void Awake()
    {
        // 1. 缓存 Button 组件，避免每次点击都重新查找。
        button = GetComponent<Button>();
    }

    #endregion

    #region 输入事件

    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. 鼠标只响应左键；触摸输入没有鼠标按键概念，需要放行。
        bool isMousePointer = eventData != null && eventData.pointerId < 0;
        if (isMousePointer && eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // 2. 按钮无效、隐藏或不可交互时，不播放点击音效。
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button == null || !button.IsActive() || !button.interactable)
        {
            return;
        }

        // 3. 如果按钮或父级带有忽略标记，则交给专属音效或保持静音。
        if (GetComponentInParent<UIButtonClickSfxIgnore>(true) != null)
        {
            return;
        }

        // 4. 通过配置表分发到对应音频通道，避免表中bus调整后代码失效。
        AudioManager.Ins.PlayAudioById(ButtonClickAudioId);
    }

    #endregion
}
