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
    void Start()
    { 
        player = GetComponent<PlayerStats>();
        lastMoveVector = new Vector2(1, 0f);
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if(GameManager.instance.isGameOver) return; 
        InputManagement();
    }
    void FixedUpdate()
    {
        Move();
    }
       void InputManagement()
    {
        // 1. 从总线读取解耦后的向量
        moveDir = InputManager.instance != null ? InputManager.instance.movementVector : Vector2.zero;

        // 2. 指令流优化：只在向量发生物理偏转时，才执行一次赋值与拆包
        if (moveDir != Vector2.zero)
        {
            // 直接将当前的非零向量锁存进内存
            lastMoveVector = moveDir; 
            
            // 如果你的其他脚本（如动画控制器）严格依赖这单独的 X 和 Y 标量，在此处进行拆包
            lastHorizontalVector = moveDir.x;
            lastVerticalVector = moveDir.y;
        }
    }
    void Move()
    {
        rb.linearVelocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }
    
}
