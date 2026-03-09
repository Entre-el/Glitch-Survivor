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
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;
        if(moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMoveVector = new Vector2(lastHorizontalVector, 0f);
        }
        if(moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMoveVector = new Vector2(0f, lastVerticalVector);
        }
        if(moveDir.x != 0 && moveDir.y != 0)
        {
            lastMoveVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }
    void Move()
    {
        rb.linearVelocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }
    
}
