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
        moveDir = InputManager.instance != null ? InputManager.instance.movementVector : Vector2.zero;

        if (moveDir != Vector2.zero)
        {
            lastMoveVector = moveDir.normalized;
            lastHorizontalVector = moveDir.x;
            lastVerticalVector = moveDir.y;
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }
}
