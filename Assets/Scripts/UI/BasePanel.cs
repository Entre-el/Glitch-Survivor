using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class BasePanel : MonoBehaviour 
{
    [SerializeField]
    protected CanvasGroup canvasGroup;
    protected bool hasInit = false;
    public virtual void OnInit() 
    { 
        if (!hasInit)
        return;
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        hasInit = true;
    }
    public virtual void OnShow() 
    { 
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true; 
    }
    public virtual void OnHide() 
    { 
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false; 
    }
}