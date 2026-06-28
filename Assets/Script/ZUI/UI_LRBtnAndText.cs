using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_LRBtnAndText : MonoBehaviour, ISelectHandler
{
    public Text text;
    public Button leftButton;
    public Button rightButton;
    public ScrollRect scrollRect;
    public void Init(UnityAction<Text> left, UnityAction<Text> right)
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(() => left(text));
        rightButton.onClick.AddListener(() => right(text));
    }

    public void LeftButtonOnClick()
    {
        leftButton.onClick.Invoke();
    }
    public void RightButtonOnClick()
    {
        rightButton.onClick.Invoke();
    }

    /// <summary>
    /// 当该UI被EventSystem选中时自动触发
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        if (scrollRect == null) return;
        FocusOnSelf();
    }
    private void FocusOnSelf()
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;
        RectTransform target = transform as RectTransform;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        if (contentHeight <= viewportHeight)
        {
            // 内容没超过视口，不需要滚动
            scrollRect.verticalNormalizedPosition = 1f;
            return;
        }

        // 因为 Pivot 在(0,1)，Y是从上往下负数
        float itemPosY = Mathf.Abs(target.anchoredPosition.y);

        // 希望目标出现在视口中间
        float scrollY = itemPosY - viewportHeight * 0.5f;

        float maxScroll = contentHeight - viewportHeight;
        float normalized = Mathf.Clamp01(scrollY / maxScroll);

        // ScrollRect：1 = 顶部，0 = 底部
        scrollRect.verticalNormalizedPosition = 1f - normalized;
    }
}