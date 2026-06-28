using UnityEngine;
using UnityEngine.UI;

public class ScrollViewFocus : MonoBehaviour
{
    public ScrollRect scrollRect;

    /// <summary>
    /// 定位到某个目标Item
    /// </summary>
    public void FocusOnItem(RectTransform target)
    {
        Canvas.ForceUpdateCanvases(); // 先强制刷新一次布局

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        // Content总高度
        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        // 目标在Content中的本地坐标
        Vector2 localPos = (Vector2)content.InverseTransformPoint(target.position);
        
        // 目标中心点Y
        float targetY = localPos.y + contentHeight * 0.5f;

        // 计算normalizedPosition
        float normalizedY = 1 - Mathf.Clamp01(
            (targetY - viewportHeight * 0.5f) / (contentHeight - viewportHeight)
        );

        scrollRect.verticalNormalizedPosition = normalizedY;
    }
}