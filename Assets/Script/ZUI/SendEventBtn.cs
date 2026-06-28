using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SendEventBtn : MonoBehaviour
{
    public EventType eventType;
    protected Button button;
    public Vector2Int input;

    public virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
            {
                EventManager.Ins.EventTrigger(eventType);
                EventSystem.current.SetSelectedGameObject(null);
            }
        );
    }
}