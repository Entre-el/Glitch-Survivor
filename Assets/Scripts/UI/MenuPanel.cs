using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : BasePanel
{
    public Button startButton;
    public Button exitButton;
    public Button instructionButton;
    public CanvasGroup instructionScreen;
    

    public override void OnShow()
    {
        instructionScreen.alpha = 0;
        instructionScreen.blocksRaycasts = false;
        instructionScreen.interactable = false;
        base.OnShow();

        // 防止多重挂载导致内存泄漏
        startButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        instructionButton.onClick.RemoveAllListeners();

        startButton.onClick.AddListener(OnStartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        instructionButton.onClick.AddListener(OnInstructionButtonClick);
    }

    private void OnStartButtonClick()
    {
        EventCenter.Broadcast(EventDefine.OnRequestSceneChange);
    }

    private void OnExitButtonClick()
    {
        GameManager.Instance.QuitGame();
    }

    private void OnInstructionButtonClick()
    {
        instructionScreen.alpha = 1;
        instructionScreen.blocksRaycasts = true;
        instructionScreen.interactable = true;
    }
}