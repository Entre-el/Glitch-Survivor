using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BasePanel : MonoBehaviour 
{
    [SerializeField]
    protected CanvasGroup canvasGroup;
    protected bool hasInit = false;

    // 🌟 1. 初始化方法：只执行一次，适合绑定按钮事件、获取组件
    public virtual void OnInit() 
    { 
        if (hasInit) return; // 修复了这里的逻辑反转

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        hasInit = true;
    }

    // 🌟 2. 显示方法：每次面板打开时调用，适合刷新数据
    public virtual void OnShow() 
    { 
        // 确保显示前已经初始化过
        if (!hasInit) OnInit(); 

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true; 
    }

    // 🌟 3. 隐藏方法：每次面板关闭时调用，适合清理临时状态
    public virtual void OnHide() 
    { 
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false; 
    }
}