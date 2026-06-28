using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class VerticalButtonNavigation : MonoBehaviour
{
    // public int chapter;
    public Button fristBtn;
    public Button lastBtn;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    private void Awake()
    {
        // 获取所有子物体中的 Button（只获取当前物体下的直接 Button）
        List<Button> buttons = GetComponentsInChildren<Button>().ToList();
        int i = 0;
        // foreach (var btn in buttons)
        // {
        //     LevelButton lbtn = btn.GetComponent<LevelButton>();
        //     if (lbtn != null&&i < buttons.Count)
        //     {
        //         lbtn.Init(chapter * 10 + i + 1);
        //         i++;
        //     }
        // }
        // 为每个按钮设置 navigation
        for (i = 0; i < buttons.Count; i++)
        {
            Navigation nav = buttons[i].navigation;
            nav.mode = Navigation.Mode.Explicit;

            // 上下导航目标
            nav.selectOnUp = (i > 0) ? buttons[i - 1] : fristBtn;
            nav.selectOnDown = (i < buttons.Count - 1) ? buttons[i + 1] : lastBtn;
            buttons[i].navigation = nav;
        }
    }
}