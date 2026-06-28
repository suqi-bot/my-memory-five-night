using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public static UIManager Instance => instance;
    private static UIManager instance = new UIManager();

    private Dictionary<string, UIPanel> panelDic = new Dictionary<string, UIPanel>();
    private Transform canvasTrans;

    private UIManager()
    {
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvas.transform;
        GameObject.DontDestroyOnLoad(canvas);
    }

    public T ShowPanel<T>() where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return (T)panelDic[panelName];

        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName), canvasTrans);
        T panel = panelObj.GetComponent<T>();
        panel.Init();
        panel.Show();
        panelDic.Add(panelName, panel);
        return panel;
    }

    public void HidePanel<T>(bool isFade = true) where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (!panelDic.ContainsKey(panelName)) return;

        T nowPanel = (T)panelDic[panelName];
        if (isFade)
        {
            nowPanel.Hide(() =>
            {
                GameObject.Destroy(nowPanel.gameObject);
                panelDic.Remove(panelName);
            });
        }
        else
        {
            nowPanel.Hide();
            GameObject.Destroy(nowPanel.gameObject);
            panelDic.Remove(panelName);
        }
    }

    public T GetPanel<T>() where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return (T)panelDic[panelName];
        return null;
    }

    public bool HasPanel<T>() where T : UIPanel
    {
        return panelDic.ContainsKey(typeof(T).Name);
    }

    public void HideAllPanels()
    {
        foreach (var panel in panelDic.Values)
        {
            if (panel != null)
                GameObject.Destroy(panel.gameObject);
        }
        panelDic.Clear();
    }
}
