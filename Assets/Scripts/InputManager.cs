using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance{ get; private set; }
    public Vector2 movementVector{ get; private set; }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of InputManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    public void SetMovementVector(Vector2 newVector)
    {
        movementVector = Vector2.ClampMagnitude(newVector,1f);   
    }
    private void Update()
    {
    #if UNITY_EDITOR || UNITY_STANDALONE
    if(movementVector == Vector2.zero)
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        SetMovementVector(new Vector2(moveX, moveY));
    }
    #endif
    }
}
