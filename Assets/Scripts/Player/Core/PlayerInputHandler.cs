using System;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerInputHandler : MonoBehaviour
{
    [Header("移动意图 (供 Locomotion 拉取)")]
    public Vector2 MoveIntent { get; private set; }

    [Header("瞄准意图 (供 WeaponBrain 拉取)")]
    public Vector2 AimIntent { get; private set; }
    // 指示当前瞄准是否使用鼠标，供外界判断是否需要将屏幕坐标转换为世界坐标
    public bool IsUsingMouse { get; private set; } 
    private GameInputActions inputActions;
    public void Initialize()
    {
        inputActions = new GameInputActions();

        // 1. 连续意图：移动
        inputActions.Player.Move.performed += ctx => MoveIntent = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => MoveIntent = Vector2.zero;

        // 2. 连续意图：瞄准与设备溯源
        inputActions.Player.Aim.performed += ctx => 
        {
            AimIntent = ctx.ReadValue<Vector2>();
        // 新输入系统的降维打击：瞬间判定当前输入是由什么物理设备触发的
            IsUsingMouse = ctx.control.device is Mouse; 
        };
        inputActions.Player.Aim.canceled += ctx => AimIntent = Vector2.zero;

        // 3. 瞬时意图：主动作 (Started 等同于 GetKeyDown, Canceled 等同于 GetKeyUp)
        inputActions.Player.Action.started += ctx => EventCenter.Broadcast(EventDefine.OnActionPressed);
        inputActions.Player.Action.canceled += ctx => EventCenter.Broadcast(EventDefine.OnActionReleased);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}