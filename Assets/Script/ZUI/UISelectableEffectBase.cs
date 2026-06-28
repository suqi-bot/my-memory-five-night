using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UISelectableEffectBase : MonoBehaviour
{
    public bool isSelect;

    [Header("图片效果")]
    public bool imageEffect;
    public Image targetImage;
    public Color normalImageColor = Color.white;
    public Color selectedImageColor = Color.white;
    public Sprite normalSprite;
    public Sprite selectedSprite;

    [Header("文本效果")]
    public bool textEffect;
    public Text text;
    public Color normalTextColor = Color.white;
    public Color selectedTextColor = Color.white;

    public GameObject[] showObjs;
    public GameObject[] hideObjs;

    protected Vector3 originalScale;
    public Vector3 selectedScale = Vector3.one;

    public Selectable selectable;

    public bool listenerEvent = true;

    protected virtual void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    public virtual void Init(UnityAction action)
    {
     
    }

    public virtual void OnClick()
    {
     
    }
    public virtual void OnSwitch(bool select)
    {
        if (select)
            OnSelect();
        else
            OnDeselect();
    }

    // 当被选中时调用
    public virtual void OnSelect(BaseEventData eventData)
    {
        if (!listenerEvent) return;
        OnSelect();
    }

    public virtual void OnSelect()
    {
        if (isSelect)
            return;

        isSelect = true;

        SetImageEffect(true);
        SetTextEffect(selectedTextColor);

        transform.localScale = selectedScale;
        SwitchObjs(true);
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (!listenerEvent) return;
        OnDeselect();
    }

    // 当取消选中时调用
    public void OnDeselect()
    {
        if (!isSelect)
            return;

        isSelect = false;
        SetImageEffect(false);
        SetTextEffect(normalTextColor);
        transform.localScale = originalScale;
        SwitchObjs(false);
    }

    public void SetImageEffect(bool select)
    {
        if (!imageEffect || targetImage == null)
            return;

        if (select)
        {
            targetImage.color = selectedImageColor;
            if (selectedSprite != null)
                targetImage.sprite = selectedSprite;
        }
        else
        {
            targetImage.color = normalImageColor;
            if (normalSprite != null)
                targetImage.sprite = normalSprite;
        }
    }

    public void SetTextEffect(Color color)
    {
        if (!textEffect || text == null)
            return;

        text.color = color;
    }

    public void SwitchObjs(bool show)
    {
        if (showObjs != null)
            foreach (var item in showObjs)
            {
                if (item != null)
                    item.SetActive(show);
            }

        if (hideObjs != null)
            foreach (var item in hideObjs)
            {
                if (item != null)
                    item.SetActive(!show);
            }
    }
}