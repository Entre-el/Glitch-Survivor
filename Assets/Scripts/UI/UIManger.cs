using UnityEngine;
using System.Collections.Generic;
using System;
public class UIManger : MonoBehaviour
{
    public static UIManger Instance;
    private Dictionary<Type, BasePanel> panelDictionary = new (20);
    private Stack<BasePanel> panelStack = new (20);
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        BasePanel[] panels = GetComponentsInChildren<BasePanel>(true);
        foreach (var panel in panels)
        {
            RegisterPanel(panel);
            if (!panel.gameObject.activeSelf)
            {
                panel.gameObject.SetActive(true);
            }
            panel.OnHide();
        }
    }
    public void RegisterPanel(BasePanel panel)
    {
        Type panelType = panel.GetType();
        if (!panelDictionary.ContainsKey(panelType))
        {
            panel.OnInit();
            panelDictionary.Add(panelType, panel);
        }
    }
    public void UnregisterPanel(BasePanel panel)
    {
        Type panelType = panel.GetType();
        if (panelDictionary.ContainsKey(panelType))
        {
            panel.OnHide();
            panelDictionary.Remove(panelType);
        }
    }
    public void ShowPanel<T>() where T : BasePanel
    {
        Type type = typeof(T);
        if (panelDictionary.TryGetValue(type, out var panel))
        {
            panel.OnShow();
            panelStack.Push(panel);
        }
        else
        {
            Debug.LogError($"UI架构严重异常：尝试打开未注册的面板 {type.Name}");
        }
    }
    public void HidePanel<T>() where T : BasePanel
    {
        Type type = typeof(T);
        if (panelDictionary.TryGetValue(type, out var panel))
        {
            panel.OnHide();

            if (panelStack.Count > 0 && panelStack.Peek() == panel)
            {
                panelStack.Pop();
            }
        }
    }
    public void PopTopPanel()
    {
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Pop();
            topPanel.OnHide();
        }
    }
    public T GetPanel<T>() where T : BasePanel
    {
        Type type = typeof(T);
        if (panelDictionary.TryGetValue(type, out var panel))
        {
            return panel as T;
        }
        else
        {
            Debug.LogError($"UI架构严重异常：尝试获取未注册的面板 {type.Name}");
        }
        return null;
    }
}
