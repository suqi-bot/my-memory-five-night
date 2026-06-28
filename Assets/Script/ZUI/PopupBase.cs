using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    public Button closeBtn;
    private void Awake()
    {
        if (closeBtn != null)
        {
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(Close);
        }
    }
    public virtual void Close()
    {
        Destroy(gameObject);
    }
}
