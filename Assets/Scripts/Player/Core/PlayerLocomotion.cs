using System.Collections;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_15 = new(0.15f);
    private Rigidbody2D rb;
    private Vector2 moveDir;

    private System.Action onActionPressedDelegate;
    private System.Action onActionReleasedDelegate;

    public bool CanUseDefaultDash { get; set; } = true;
    private float lastDashTime;
    private float dashEndTime;
    private PlayerCore core;
    private float moveSpeed;
    private bool tryingDash = false;

    public void Initialize(PlayerCore core)
    {
        this.core = core;
        TryGetComponent(out rb);
        moveSpeed = core.Stats.PlayerMoveSpeed.Value;

        // 初始化默认行为
        onActionPressedDelegate = DefaultOnActionPressed;
        onActionReleasedDelegate = DefaultOnActionReleased;

        EventCenter.AddListener(EventDefine.OnActionPressed, HandleActionPressed);
        EventCenter.AddListener(EventDefine.OnActionReleased, HandleActionReleased);
    }

    private void FixedUpdate()
    {
        moveDir = core.InputHandler != null ? core.InputHandler.MoveIntent : Vector2.zero;

        if (Time.time > dashEndTime && !tryingDash)
        {
            Move();
        }
        else if (tryingDash)
        {
            Move(0.5f); // 冲刺时的移动速度可以稍微降低，增加操作感
        }
    }

    private void HandleActionPressed() => onActionPressedDelegate?.Invoke();

    private void HandleActionReleased() => onActionReleasedDelegate?.Invoke();

    private void DefaultOnActionPressed()
    {
        tryingDash = true;
    }

    // 默认的松开动作现在不需要做任何事
    private void DefaultOnActionReleased()
    {
        tryingDash = false; // 取消尝试冲刺的状态
        if (CanUseDefaultDash && Time.time >= lastDashTime + core.Stats.PlayerDashCooldown.Value)
        {
            lastDashTime = Time.time;

            // 额外防御：如果玩家没有按方向键，默认往前冲（或者不冲刺，根据你的设计）
            if (moveDir == Vector2.zero)
                return;
            Dash(3f);
            StartCoroutine(DashEndCoroutine());
        }
    }

    private IEnumerator DashEndCoroutine()
    {
        yield return _waitForSeconds0_15;
        EventCenter.Broadcast(EventDefine.OnPlayerDashEnd);
    }

    private void Move(float overrideSpeed = 1f)
    {
        rb.linearVelocity = moveSpeed * overrideSpeed * moveDir;
    }

    private void Dash(float dashSpeed = 3f)
    {
        dashEndTime = Time.time + 0.15f;
        EventCenter.Broadcast(EventDefine.OnPlayerDashed);

        rb.linearVelocity = dashSpeed * moveSpeed * moveDir.normalized;
    }

    // 🌟 修复 1：利用 ?? 运算符。如果传入 null，立刻恢复默认的委托！
    public void SetDashOverride(System.Action onActionPressed, System.Action onActionReleased)
    {
        onActionPressedDelegate = onActionPressed ?? DefaultOnActionPressed;
        onActionReleasedDelegate = onActionReleased ?? DefaultOnActionReleased;
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.OnActionPressed, HandleActionPressed);
        EventCenter.RemoveListener(EventDefine.OnActionReleased, HandleActionReleased);
    }
}
