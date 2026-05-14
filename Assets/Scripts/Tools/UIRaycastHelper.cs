using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIRaycastHelper : MonoBehaviour
{
    // 全局单例
    public static UIRaycastHelper Instance;

    private void Awake()
    {
        // 确保全宇宙只有一个雷达，并且切换场景时不销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 🌟 新输入系统：监听 X 键按下
        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            ScanUIUnderMouse();
        }
    }

    /// <summary>
    /// 核心探测逻辑与 Debug 输出
    /// </summary>
    private void ScanUIUnderMouse()
    {
        List<RaycastResult> results = GetUIElementsUnderPointer();

        if (results.Count == 0)
        {
            Debug.Log(
                "<color=#888888>【UI雷达】当前鼠标下方非常干净，没有任何 UI 拦截射线。</color>"
            );
            return;
        }

        // 使用 StringBuilder 拼接，优化性能
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(
            $"<color=cyan>【UI雷达】按下 X 键！在鼠标坐标 {Pointer.current.position.ReadValue()} 处发现了 {results.Count} 层 UI：</color>"
        );

        for (int i = 0; i < results.Count; i++)
        {
            GameObject obj = results[i].gameObject;
            string fullPath = GetGameObjectPath(obj.transform);

            if (i == 0)
            {
                // 第 0 层是最顶层的，也就是直接吃掉你鼠标点击的“头号嫌疑犯”
                sb.AppendLine(
                    $"<color=red>► [最顶层拦截] <b>{obj.name}</b></color> (路径: {fullPath})"
                );
            }
            else
            {
                // 下面的层是被盖住的无辜群众
                sb.AppendLine($"  - [第 {i} 层] {obj.name} (路径: {fullPath})");
            }
        }

        //Debug.Log(sb.ToString());
    }

    /// <summary>
    /// 底层射线获取方法
    /// </summary>
    public static List<RaycastResult> GetUIElementsUnderPointer()
    {
        List<RaycastResult> results = new List<RaycastResult>();

        if (EventSystem.current == null || Pointer.current == null)
            return results;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Pointer.current.position.ReadValue(),
        };

        EventSystem.current.RaycastAll(eventData, results);
        return results;
    }

    /// <summary>
    /// 辅助方法：获取物体在 Hierarchy 中的完整路径，方便找人
    /// </summary>
    private string GetGameObjectPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}
