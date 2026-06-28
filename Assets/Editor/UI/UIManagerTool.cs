using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class UIManagerTool : EditorWindow
{
    private enum Tab
    {
        Create,
        List,
        Settings
    }

    private Tab currentTab = Tab.Create;
    private string newPanelName = "NewPanel";
    private string panelDescription = "";
    private UIPanelType panelType = UIPanelType.其它;
    private bool createPrefab = true;
    private bool addToResources = true;
    private bool enableFade = false;

    private Vector2 listScrollPos;
    private Vector2 settingsScrollPos;
    private List<PanelInfo> panelList = new List<PanelInfo>();
    private int selectedPanelIndex = -1;

    private string scriptPath = "Assets/Script/UI";
    private string prefabPath = "Assets/Resources/UI";
    private string templatePath = "Assets/Editor/UI/Templates";

    private class PanelInfo
    {
        public string name;
        public string scriptPath;
        public string prefabPath;
        public UIPanelType panelType;
        public bool hasPrefab;
    }

    [MenuItem("Tools/UI Manager Tool")]
    public static void ShowWindow()
    {
        var window = GetWindow<UIManagerTool>("UI管理工具");
        window.minSize = new Vector2(450, 500);
    }

    private void OnEnable()
    {
        RefreshPanelList();
    }

    private void OnGUI()
    {
        DrawToolbar();
        EditorGUILayout.Space(5);

        switch (currentTab)
        {
            case Tab.Create:
                DrawCreateTab();
                break;
            case Tab.List:
                DrawListTab();
                break;
            case Tab.Settings:
                DrawSettingsTab();
                break;
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            if (GUILayout.Toggle(currentTab == Tab.Create, "创建面板", EditorStyles.toolbarButton))
                currentTab = Tab.Create;
            if (GUILayout.Toggle(currentTab == Tab.List, "面板列表", EditorStyles.toolbarButton))
                currentTab = Tab.List;
            if (GUILayout.Toggle(currentTab == Tab.Settings, "设置", EditorStyles.toolbarButton))
                currentTab = Tab.Settings;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawCreateTab()
    {
        EditorGUILayout.LabelField("创建新UI面板", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        newPanelName = EditorGUILayout.TextField("面板名称", newPanelName);
        panelDescription = EditorGUILayout.TextField("面板描述", panelDescription);
        panelType = (UIPanelType)EditorGUILayout.EnumPopup("面板类型", panelType);

        EditorGUILayout.Space(5);
        createPrefab = EditorGUILayout.Toggle("创建预制体", createPrefab);
        addToResources = EditorGUILayout.Toggle("添加到Resources", addToResources);
        enableFade = EditorGUILayout.Toggle("启用淡入淡出", enableFade);

        EditorGUILayout.Space(10);

        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(newPanelName));
        if (GUILayout.Button("创建面板", GUILayout.Height(35)))
        {
            CreatePanel();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(10);
        DrawPreview();
    }

    private void DrawPreview()
    {
        EditorGUILayout.LabelField("预览", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            $"脚本路径: {scriptPath}/{newPanelName}.cs\n" +
            (createPrefab ? $"预制体路径: {(addToResources ? prefabPath : "Assets")}/{newPanelName}.prefab\n" : "") +
            $"基类: UIPanel\n" +
            $"面板类型: {panelType}\n" +
            $"淡入淡出: {(enableFade ? "启用" : "禁用")}",
            MessageType.Info
        );
    }

    private void DrawListTab()
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("UI面板列表", EditorStyles.boldLabel);
            if (GUILayout.Button("刷新", GUILayout.Width(60)))
            {
                RefreshPanelList();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        if (panelList.Count == 0)
        {
            EditorGUILayout.HelpBox("未找到UI面板脚本", MessageType.Info);
            return;
        }

        listScrollPos = EditorGUILayout.BeginScrollView(listScrollPos);
        {
            for (int i = 0; i < panelList.Count; i++)
            {
                DrawPanelItem(i);
            }
        }
        EditorGUILayout.EndScrollView();

        if (selectedPanelIndex >= 0 && selectedPanelIndex < panelList.Count)
        {
            DrawSelectedPanelActions();
        }
    }

    private void DrawPanelItem(int index)
    {
        var panel = panelList[index];
        bool isSelected = selectedPanelIndex == index;

        EditorGUILayout.BeginHorizontal();
        {
            Color bgColor = GUI.backgroundColor;
            if (isSelected) GUI.backgroundColor = new Color(0.5f, 0.8f, 1f);

            if (GUILayout.Button("", GUILayout.Width(20)))
            {
                selectedPanelIndex = index;
            }

            GUI.backgroundColor = bgColor;

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(panel.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"  类型: {panel.panelType} | 路径: {panel.scriptPath}", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();

            if (panel.hasPrefab)
            {
                if (GUILayout.Button("预制体", GUILayout.Width(50)))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(panel.prefabPath);
                    if (prefab != null) Selection.activeObject = prefab;
                }
            }

            if (GUILayout.Button("脚本", GUILayout.Width(50)))
            {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(panel.scriptPath);
                if (script != null) AssetDatabase.OpenAsset(script);
            }
        }
        EditorGUILayout.EndHorizontal();

        if (index < panelList.Count - 1)
            EditorGUILayout.Space(2);
    }

    private void DrawSelectedPanelActions()
    {
        var panel = panelList[selectedPanelIndex];

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("操作", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("选中脚本"))
            {
                var script = AssetDatabase.LoadAssetAtPath<Object>(panel.scriptPath);
                if (script != null) Selection.activeObject = script;
            }

            if (GUILayout.Button("选中预制体"))
            {
                if (panel.hasPrefab)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<Object>(panel.prefabPath);
                    if (prefab != null) Selection.activeObject = prefab;
                }
            }

            if (GUILayout.Button("删除"))
            {
                if (EditorUtility.DisplayDialog("确认删除", $"确定要删除 {panel.name} 吗？", "删除", "取消"))
                {
                    DeletePanel(panel);
                    RefreshPanelList();
                    selectedPanelIndex = -1;
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSettingsTab()
    {
        settingsScrollPos = EditorGUILayout.BeginScrollView(settingsScrollPos);
        {
            EditorGUILayout.LabelField("路径设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            scriptPath = EditorGUILayout.TextField("脚本路径", scriptPath);
            prefabPath = EditorGUILayout.TextField("预制体路径", prefabPath);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("模板设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("创建模板文件夹"))
            {
                CreateTemplateFolder();
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "模板文件说明:\n" +
                "- UIPanel模板: Assets/Editor/UI/Templates/UIPanelTemplate.txt\n\n" +
                "可用变量:\n" +
                "{PANEL_NAME} - 面板名称\n" +
                "{PANEL_DESC} - 面板描述\n" +
                "{PANEL_TYPE} - 面板类型",
                MessageType.Info
            );
        }
        EditorGUILayout.EndScrollView();
    }

    private void CreatePanel()
    {
        if (string.IsNullOrEmpty(newPanelName))
        {
            EditorUtility.DisplayDialog("错误", "请输入面板名称", "确定");
            return;
        }

        if (!Directory.Exists(scriptPath))
        {
            Directory.CreateDirectory(scriptPath);
        }

        string scriptFilePath = $"{scriptPath}/{newPanelName}.cs";

        if (File.Exists(scriptFilePath))
        {
            if (!EditorUtility.DisplayDialog("文件已存在", $"脚本 {newPanelName}.cs 已存在，是否覆盖？", "覆盖", "取消"))
                return;
        }

        string scriptContent = GenerateScriptContent();
        File.WriteAllText(scriptFilePath, scriptContent);
        AssetDatabase.Refresh();

        if (createPrefab)
        {
            CreatePanelPrefab();
        }

        EditorUtility.DisplayDialog("成功", $"面板 {newPanelName} 创建成功！", "确定");

        RefreshPanelList();
        currentTab = Tab.List;
    }

    private string GenerateScriptContent()
    {
        string template = GetTemplate();

        template = template.Replace("{PANEL_NAME}", newPanelName);
        template = template.Replace("{PANEL_DESC}", panelDescription);
        template = template.Replace("{PANEL_TYPE}", panelType.ToString());

        return template;
    }

    private string GetTemplate()
    {
        string templateFile = $"{templatePath}/UIPanelTemplate.txt";

        if (File.Exists(templateFile))
        {
            return File.ReadAllText(templateFile);
        }

        return GetDefaultUIPanelTemplate();
    }

    private string GetDefaultUIPanelTemplate()
    {
        return @"using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// {PANEL_DESC}
/// </summary>
public class {PANEL_NAME} : UIPanel
{
    // 在Inspector中设置UI组件引用
    // public Button closeButton;
    // public Text titleText;

    public override void Init(object data = null)
    {
        base.Init(data);
        // 初始化逻辑
    }

    public override void Show()
    {
        base.Show();
        // 显示时的逻辑
    }

    public override void Hide()
    {
        base.Hide();
        // 隐藏时的逻辑
    }

    public override void ResetPanel()
    {
        base.ResetPanel();
        // 重置面板状态
    }
}
";
    }

    private void CreatePanelPrefab()
    {
        string targetPath = addToResources ? prefabPath : "Assets";

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        GameObject panelObj = new GameObject(newPanelName);
        RectTransform rectTransform = panelObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        CanvasGroup canvasGroup = panelObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        string scriptFilePath = $"{scriptPath}/{newPanelName}.cs";
        if (File.Exists(scriptFilePath))
        {
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptFilePath);
            if (script != null)
            {
                panelObj.AddComponent(script.GetClass());
            }
        }

        string prefabFilePath = $"{targetPath}/{newPanelName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(panelObj, prefabFilePath);
        DestroyImmediate(panelObj);

        AssetDatabase.Refresh();
    }

    private void CreateTemplateFolder()
    {
        if (!Directory.Exists(templatePath))
        {
            Directory.CreateDirectory(templatePath);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("成功", "模板文件夹已创建", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("提示", "模板文件夹已存在", "确定");
        }
    }

    private void RefreshPanelList()
    {
        panelList.Clear();

        if (!Directory.Exists(scriptPath))
        {
            Directory.CreateDirectory(scriptPath);
            AssetDatabase.Refresh();
            return;
        }

        string[] scriptFiles = Directory.GetFiles(scriptPath, "*.cs", SearchOption.AllDirectories);

        foreach (string file in scriptFiles)
        {
            string content = File.ReadAllText(file);
            string fileName = Path.GetFileNameWithoutExtension(file);

            if (!content.Contains(": UIPanel"))
            {
                continue;
            }

            string prefabPathFull = $"{prefabPath}/{fileName}.prefab";
            bool hasPrefab = File.Exists(prefabPathFull);

            UIPanelType panelTypeValue = UIPanelType.其它;
            var typeMatch = System.Text.RegularExpressions.Regex.Match(content, @"panelType\s*=\s*UIPanelType\.(\w+)");
            if (typeMatch.Success)
            {
                System.Enum.TryParse(typeMatch.Groups[1].Value, out panelTypeValue);
            }

            panelList.Add(new PanelInfo
            {
                name = fileName,
                scriptPath = file.Replace("\\", "/"),
                prefabPath = hasPrefab ? prefabPathFull : "",
                panelType = panelTypeValue,
                hasPrefab = hasPrefab
            });
        }

        panelList = panelList.OrderBy(p => p.name).ToList();
    }

    private void DeletePanel(PanelInfo panel)
    {
        if (File.Exists(panel.scriptPath))
        {
            File.Delete(panel.scriptPath);
            File.Delete(panel.scriptPath + ".meta");
        }

        if (panel.hasPrefab && File.Exists(panel.prefabPath))
        {
            File.Delete(panel.prefabPath);
            File.Delete(panel.prefabPath + ".meta");
        }

        AssetDatabase.Refresh();
    }
}
