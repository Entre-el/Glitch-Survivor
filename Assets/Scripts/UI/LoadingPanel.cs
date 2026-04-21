using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingPanel : BasePanel
{
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;

    // 极其暴力的开放接口，只做表现！
    public void UpdateProgress(float progress, string log)
    {
        loadingSlider.value = progress;
        loadingText.SetText($"> {log}");
    }
}