using UnityEngine;
using UnityEngine.UI;
public class MenuPanel : BasePanel
{
    public Button startButton;
    public Button exitButton;
    public Button instructionButton;
    public CanvasGroup instructionScreen;
    public SceneSO nextScene;
    public override void OnShow()
    {
        instructionScreen.alpha = 0;
        instructionScreen.blocksRaycasts = false;
        instructionScreen.interactable = false;
        base.OnShow();
        startButton.onClick.AddListener(OnStartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        instructionButton.onClick.AddListener(OnInstructionButtonClick);
    }
    private void OnStartButtonClick()
    {
        EventCenter.Broadcast(EventDefine.OnRequestSceneChange, nextScene);
    }
    private void OnExitButtonClick()
    {
        GameManager.instance.QuitGame();
    }
    private void OnInstructionButtonClick()
    {
        instructionScreen.alpha = 1;
        instructionScreen.blocksRaycasts = true;
        instructionScreen.interactable = true;
    }
}
