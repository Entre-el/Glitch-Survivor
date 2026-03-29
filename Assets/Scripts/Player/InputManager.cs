using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("移动数据 (左摇杆/键盘)")]
    public Vector2 movementVector { get; private set; }
    public bool isMoveJoystickActive { get; set; } = false;

    [Header("瞄准数据 (右摇杆/鼠标)")]
    public Vector2 aimVector { get; private set; }
    public bool isAimJoystickActive { get; set; } = false;
    public bool isUsingMouseToAim { get; private set; } = true; 

    private void Awake()
    {
        if(Instance is null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetMovementVector(Vector2 newVector)
    {
        movementVector = Vector2.ClampMagnitude(newVector, 1f);   
    }

    public void SetAimVector(Vector2 newVector)
    {
        // 只要右摇杆一动，就说明玩家在用手机玩
        if (newVector != Vector2.zero)
        {
            aimVector = newVector.normalized;
            isUsingMouseToAim = false; 
        }
    }

    private void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        if(!isMoveJoystickActive)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            SetMovementVector(new Vector2(moveX, moveY).normalized);
        }
        
        if (!isAimJoystickActive && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
        {
            isUsingMouseToAim = true;
        }
        #endif
    }
}