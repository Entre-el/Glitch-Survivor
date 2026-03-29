using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMoveVector;
    PlayerStats player;

    [Header("武器/准星旋转")]
    public Transform weaponPivot;
    public Transform pivotIcon;
    [Tooltip("摇杆模式下，准星距离主角的固定长度")]
    public float aimIconDistance = 4f;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        player = GetComponent<PlayerStats>();
        lastMoveVector = new Vector2(1, 0f);
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(GameManager.Instance.isGameOver) return;
        InputManagement();
        AimManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        moveDir = InputManager.Instance != null ? InputManager.Instance.movementVector : Vector2.zero;

        if (moveDir != Vector2.zero)
        {
            lastMoveVector = moveDir.normalized;
            lastHorizontalVector = moveDir.x;
            lastVerticalVector = moveDir.y;
        }
    }

    void AimManagement()
    {
        Vector2 finalAimDirection = Vector2.right; // 默认朝右

        // 1. 获取输入方向
        if (InputManager.Instance.isUsingMouseToAim)
        {
            Vector3 playerScreenPos = mainCam.WorldToScreenPoint(transform.position);
            Vector3 mousePos = Input.mousePosition;
            finalAimDirection = new Vector2(mousePos.x - playerScreenPos.x, mousePos.y - playerScreenPos.y).normalized;
        }
        else
        {
            finalAimDirection = InputManager.Instance.aimVector;
        }

        // 2. 处理旋转与准星位置
        if (finalAimDirection != Vector2.zero && !float.IsNaN(finalAimDirection.x) && !float.IsNaN(finalAimDirection.y))
        {
            float angle = Mathf.Atan2(finalAimDirection.y, finalAimDirection.x) * Mathf.Rad2Deg;
            
            if (!float.IsNaN(angle))
            {
                // 让武器轴心旋转
                weaponPivot.rotation = Quaternion.Euler(0, 0, angle);
                // 锁定准星自身角度，防止贴图跟着乱转
                pivotIcon.rotation = Quaternion.identity; 
            }

            // 处理准星的空间坐标
            if (InputManager.Instance.isUsingMouseToAim)
            {
                // 鼠标模式：将屏幕的像素坐标转化为物理世界的绝对坐标
                Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0f; 
                pivotIcon.position = mouseWorldPos;
            }
            else
            {
                // 摇杆模式：基于主角坐标，沿着瞄准方向推进固定的距离
                pivotIcon.position = transform.position + (Vector3)(finalAimDirection * aimIconDistance);
            }
            
            lastMoveVector = finalAimDirection; 
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }
}