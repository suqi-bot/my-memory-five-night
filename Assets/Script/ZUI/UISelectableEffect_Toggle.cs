using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISelectableEffect_Toggle : UISelectableEffectBase
{
    public Toggle toggle;
    protected override void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        originalScale = transform.localScale;

        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnSwitch);
            listenerEvent = false;
        }
    }
    // 当被选中时调用
    public override void OnSelect(BaseEventData eventData)
    {
    }
    public override void OnDeselect(BaseEventData eventData)
    {
    }
}