using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UIPanelType
{
    其它, 失败, 通关, 快速选关, 章节, 教学, 操作选择, 故事线, 章节结束, QTE, 帮助, 故事线_章节, 成就,
    QTE_点击, QTE_滑动, QTE_稳定控制, 档案, 循环节点_UI面板,档案_选择头像, 监控面板
}
public class UIPanel : MonoBehaviour
{
    [Header("面板设置")]
    [SerializeField] public bool allowAutoDestroy = false;
    public UIPanelType panelType = UIPanelType.其它;
    public Button closeBtn;
    public Button bgCloseBtn;

    [Header("淡入淡出")]
    [SerializeField] private float alphaSpeed = 10f;
    [SerializeField] private bool enableFade = false;

    public bool isShow;
    protected CanvasGroup canvasGroup;
    private UnityAction hideCallBack;

    protected virtual void OnEnable()
    {
        EventManager.Ins.AddEventListener<UIPanelType, bool>(EventType.TryShowUIPanel, TryShow);
        EventManager.Ins.AddEventListener<UIPanelType>(EventType.TryHideUIPanel, TryHide);
        EventManager.Ins.AddEventListener<UIPanelType, bool>(EventType.SwitchUIPanel, Switch);
        EventManager.Ins.AddEventListener<UIPanelType, bool, Action>(EventType.UITryDO, UITryDO);
    }

    protected virtual void OnDisable()
    {
        EventManager.Ins.RemoveEventListener<UIPanelType, bool>(EventType.TryShowUIPanel, TryShow);
        EventManager.Ins.RemoveEventListener<UIPanelType>(EventType.TryHideUIPanel, TryHide);
        EventManager.Ins.RemoveEventListener<UIPanelType, bool>(EventType.SwitchUIPanel, Switch);
        EventManager.Ins.RemoveEventListener<UIPanelType, bool, Action>(EventType.UITryDO, UITryDO);
    }

    protected virtual void Awake()
    {
        if (!transform.TryGetComponent<CanvasGroup>(out canvasGroup))
            canvasGroup = transform.AddComponent<CanvasGroup>();
        InitCloseBtn();
    }

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

    public virtual void Init(object data = null) { }

    public virtual void InitCloseBtn()
    {
        if (closeBtn != null)
            closeBtn.onClick.AddListener(HideSelf);
        if (bgCloseBtn != null)
            bgCloseBtn.onClick.AddListener(HideSelf);
    }

    public void HideSelf()
    {
        EventManager.Ins.EventTrigger<UIPanelType>(EventType.HidePanel, panelType);
    }

    public virtual void TryShow(UIPanelType type, bool setAsLastSibling)
    {
        if (panelType == UIPanelType.其它) return;
        if (panelType != type) return;
        Show(setAsLastSibling);
    }

    public void TryHide(UIPanelType type)
    {
        if (panelType != type) return;
        Hide();
    }

    public virtual void Switch(bool show, bool setAsLastSibling)
    {
        if (show) Show(setAsLastSibling);
        else Hide();
    }

    public virtual void Switch(UIPanelType type, bool setAsLastSibling)
    {
        if (isShow) Hide();
        else Show(setAsLastSibling);
    }

    public virtual void Switch(bool setAsLastSibling = false)
    {
        Switch(panelType, setAsLastSibling);
    }

    public virtual void SetShow(bool show, bool setAsLastSibling = false)
    {
        if (show)
        {
            if (!isShow) Show(setAsLastSibling);
        }
        else
        {
            if (isShow) Hide();
        }
    }

    public virtual void Show(bool setAsLastSibling)
    {
        Show();
        if (setAsLastSibling)
            transform.SetAsLastSibling();
    }

    public virtual void Show()
    {
        ResetPanel();
        isShow = true;

        if (enableFade)
        {
            canvasGroup.alpha = 0;
        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }

        EventManager.Ins.EventTrigger(EventType.GameUIShow);
    }

    public virtual void Hide()
    {
        isShow = false;

        if (!enableFade)
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            OnHideComplete();
        }

        EventManager.Ins.EventTrigger(EventType.GameUIHide);
        EventManager.Ins.EventTrigger<UIPanel>(EventType.RemovePanel, this);
    }

    public void Hide(UnityAction callBack)
    {
        hideCallBack = callBack;
        Hide();
    }

    private void OnHideComplete()
    {
        hideCallBack?.Invoke();
        hideCallBack = null;

        if (allowAutoDestroy)
            Destroy(gameObject);
    }

    public void InitSwitch()
    {
        if (isShow)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public virtual void ResetPanel() { }

    public virtual void UITryDO(UIPanelType type, bool isShow, Action action)
    {
        if (panelType != type) return;
        if (this.isShow == isShow)
            action?.Invoke();
    }
}
