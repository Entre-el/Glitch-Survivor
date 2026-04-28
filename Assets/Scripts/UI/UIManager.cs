using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private readonly Dictionary<Type, BasePanel> panelDictionary = new(20);
    private readonly Stack<BasePanel> panelStack = new(20);

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 🌟 核心：统一管理时间暂停与物理剥离
    private void UpdateTimeScale()
    {
        if (panelStack.Count > 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        // 全局 ESC 返回逻辑
        if (Input.GetKeyDown(KeyCode.Escape))
            PopTopPanel();
    }

    public void RegisterAndHidePanel(BasePanel panel)
    {
        Type panelType = panel.GetType();
        if (!panelDictionary.ContainsKey(panelType))
        {
            panel.OnInit();
            panelDictionary.Add(panelType, panel);
        }
        if (!panel.gameObject.activeSelf)
        {
            panel.gameObject.SetActive(true);
        }
        panel.OnHide();
    }

    public void UnregisterPanel(BasePanel panel)
    {
        Type panelType = panel.GetType();
        if (panelDictionary.ContainsKey(panelType))
        {
            panelDictionary.Remove(panelType);
        }
    }

    public void TogglePanel<T>()
        where T : BasePanel
    {
        Type type = typeof(T);
        if (panelDictionary.TryGetValue(type, out var panel))
        {
            // 判断当前面板是否在显示中
            if (panel.GetComponent<CanvasGroup>().alpha > 0)
                HidePanel<T>();
            else
                ShowPanel<T>();
        }
    }

    public void ShowPanel<T>(bool pushToStack = true)
        where T : BasePanel
    {
        Type type = typeof(T);
        if (panelDictionary.TryGetValue(type, out var panel))
        {
            panel.OnShow();

            // 🌟 如果是 Tooltip 这种轻量级浮窗，调用 ShowPanel<Tooltip>(false) 即可不压栈
            if (pushToStack)
            {
                panelStack.Push(panel);
                UpdateTimeScale(); // 更新游戏暂停状态
            }
        }
        else
        {
            Debug.LogError($"UI架构异常：尝试打开未注册的面板 {type.Name}");
        }
    }

    public void HidePanel<T>()
        where T : BasePanel
    {
        Type type = typeof(T);
        if (panelDictionary.TryGetValue(type, out var panel))
        {
            panel.OnHide();

            // 只有当要隐藏的面板刚好在栈顶时，才进行 Pop
            if (panelStack.Count > 0 && panelStack.Peek() == panel)
            {
                panelStack.Pop();
                UpdateTimeScale(); // 更新游戏暂停状态
            }
        }
    }

    public void PopTopPanel()
    {
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Pop();
            topPanel.OnHide();
            UpdateTimeScale(); // 更新游戏暂停状态
        }
    }

    public T GetPanel<T>()
        where T : BasePanel
    {
        Type type = typeof(T);
        if (panelDictionary.TryGetValue(type, out var panel))
        {
            return panel as T;
        }

        Debug.LogError($"UI架构异常：尝试获取未注册的面板 {type.Name}");
        return null;
    }
}
