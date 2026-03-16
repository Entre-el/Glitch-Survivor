using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public enum JoystickType { Movement, Aiming }
    [Header("摇杆类型")]
    public JoystickType joystickType;

    [Header("UI References")]
    public RectTransform backgroundRect;
    public RectTransform handleRect;
    private Canvas parentCanvas;
    private float magnitudeRadius;

    private void Start()
    {
        magnitudeRadius = backgroundRect.sizeDelta.x / 2f;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(InputManager.instance != null)
        {
            // 根据身份，激活不同的输入通道
            if (joystickType == JoystickType.Movement)
                InputManager.instance.isMoveJoystickActive = true;
            else
                InputManager.instance.isAimJoystickActive = true;
        }
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        Camera cam = null;
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            cam = eventData.pressEventCamera;
        }
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundRect, eventData.position, cam, out localPoint))
        {
            Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, magnitudeRadius);
            handleRect.anchoredPosition = clampedPosition;
            
            Vector2 normalizedInput = Vector2.zero;
            if (magnitudeRadius > 0.001f) 
            {
                normalizedInput = clampedPosition / magnitudeRadius;
            }
            
            if (joystickType == JoystickType.Movement)
                InputManager.instance.SetMovementVector(normalizedInput);
            else
                InputManager.instance.SetAimVector(normalizedInput);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handleRect.anchoredPosition = Vector2.zero;
        if(InputManager.instance != null) 
        {
            if (joystickType == JoystickType.Movement)
            {
                InputManager.instance.SetMovementVector(Vector2.zero);
                InputManager.instance.isMoveJoystickActive = false; 
            }
            else
            {
                InputManager.instance.isAimJoystickActive = false; 
            }
        }
    }
}