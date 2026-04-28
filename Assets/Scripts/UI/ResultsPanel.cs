using UnityEngine;
public class ResultsPanel : BasePanel
{
    public override void OnInit()
    {
        base.OnInit();
        gameObject.SetActive(false);
    }
    public override void OnShow()
    {
        base.OnShow();
        Time.timeScale = 0f;
    }
    public override void OnHide()
    {
        base.OnHide();
        Time.timeScale = 1f;
    }
}