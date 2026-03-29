using UnityEngine;
[RequireComponent(typeof(CanvasGroup))]
public class BasePanel : MonoBehaviour 
{
    [SerializeField]
    protected CanvasGroup canvasGroup;
    protected bool hasInit = false;
    public virtual void Awake()
    {
        if (!hasInit)
        {
            UIManger.Instance.RegisterPanel(this);
            OnInit();
            hasInit = true;
        }
    }
    public virtual void OnInit() 
    { 
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
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