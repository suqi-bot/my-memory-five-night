using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(Button))]
public class UISelectableEffect_Button : UISelectableEffectBase, ISelectHandler, IDeselectHandler
{
    public Button button;

    protected override void Awake()
    {
        base.Awake();

        if (button == null)
            button = GetComponent<Button>();
    }

    public override void Init(UnityAction action)
    {
        if (button == null || action == null)
            return;

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    public override void OnClick()
    {
        button?.onClick.Invoke();
    }
}