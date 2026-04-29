using UnityEngine;

// 建议改名为 CrosshairPanel (十字准星面板)，因为 WeaponPivot 很容易和武器轴心混淆
public class CrosshairPanel : BasePanel
{
    [Header("UI 绑定")]
    [Tooltip("准星的图片 (RectTransform)")]
    public RectTransform crosshairUI;

    [Header("手柄模式设置")]
    [Tooltip("手柄瞄准时，准星距离玩家屏幕中心的固定半径 (像素)")]
    public float gamepadRadius = 150f;

    // 这个依赖建议由外部 GameManager 注入，或者通过全局单例获取
    // 假设你能通过某种方式拿到当前的 PlayerCore
    public PlayerCore playerCore;

    private void Update()
    {
        // 1. 防御性检查：如果没有绑定玩家，或者玩家死了，或者 UI 没绑定，直接跳过
        if (playerCore == null || playerCore.InputHandler == null || crosshairUI == null)
            return;

        // 2. 根据不同的输入设备，采取完全不同的准星逻辑
        if (playerCore.InputHandler.IsUsingMouse)
        {
            UpdateMouseCrosshair();
        }
        else
        {
            UpdateGamepadCrosshair();
        }
    }

    // 鼠标模式
    private void UpdateMouseCrosshair()
    {
        // 注意：这里假设你的 Canvas 是 Screen Space - Overlay
        // 如果是 Camera 模式，需要用 RectTransformUtility 转换
        crosshairUI.position = Input.mousePosition;
    }

    // 手柄模式
    private void UpdateGamepadCrosshair()
    {
        Vector2 aimIntent = playerCore.InputHandler.AimIntent;

        // 如果玩家完全没推摇杆，准星可以隐藏或者保持在最后的位置
        if (aimIntent.sqrMagnitude < 0.01f)
            return;
        if (Camera.main != null)
        {
            // 以玩家屏幕位置为圆心，加上摇杆方向 * 固定像素半径
            Vector2 targetScreenPos = aimIntent.normalized * gamepadRadius;

            // 更新准星 UI 位置
            crosshairUI.position = targetScreenPos;
        }
    }
}
