using UnityEngine;
using UnityEngine.UI;

public class ExpBarPanel : BasePanel
{
    [SerializeField]
    private PlayerExperience playerExperience;

    public override void OnInit()
    {
        ;
        if (playerExperience == null)
        {
            if (!TryGetComponent<PlayerExperience>(out playerExperience))
                Debug.LogError("PlayerExp component not found");
            return;
        }
        base.OnInit();
    }

    public Slider exSlider;
    public Text LevelText;

    public void Update()
    {
        if (playerExperience != null)
        {
            exSlider.value = playerExperience.CurrentExp / playerExperience.ExpToNextLevel;
        }
    }
}
