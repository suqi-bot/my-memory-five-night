using UnityEngine;
using UnityEngine.UI;

public class LinkButton : MonoBehaviour
{
    public string url = "https://www.google.com";

    public virtual void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OpenURL);
    }

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}