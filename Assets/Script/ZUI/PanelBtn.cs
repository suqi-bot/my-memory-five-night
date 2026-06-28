using UnityEngine;
using UnityEngine.UI;

public class PanelBtn : MonoBehaviour
{
    public UIPanelType panelType= UIPanelType.其它;
    protected Button btn;
    //不通过UImanager管理
    public bool isIndependent;

    [Header("UIPanel(优先,可空)")] public UIPanel panel;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

    }
    public void OnClick() {
        if (panel != null)
        {
            panel.Switch();
        }
        else {

            if (isIndependent)
                EventManager.Ins.EventTrigger<UIPanelType>(EventType.TryShowUIPanel, panelType);
            else
                EventManager.Ins.EventTrigger<UIPanelType,object>(EventType.ShowPanel, panelType,null);
        }
    }
}
