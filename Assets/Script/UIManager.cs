
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public static UIManager Instance => instance;
    private static UIManager instance = new UIManager();

    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    private Transform canvasTrans;

    private UIManager()
    {
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvas.transform;

        //不移除canvas
        GameObject.DontDestroyOnLoad(canvas);
    }

    /// <summary>
    /// 展示面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T ShowPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return (T)panelDic[panelName];
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName), canvasTrans);

        T panel = panelObj.GetComponent<T>();
        panel.ShowMe();
        panelDic.Add(panelName, panel);
        return panel;
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void HidePanel<T>(bool isFade = true) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        T nowPanel;
        if (panelDic.ContainsKey(panelName))
        {
            nowPanel = (T)panelDic[panelName];
            if (isFade)
            {
                nowPanel.HideMe(() =>
                {
                    GameObject.Destroy(nowPanel);
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                GameObject.Destroy(nowPanel);
                panelDic.Remove(panelName);
            }
        }
        
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return (T)panelDic[panelName];
        return null;
    }

}
