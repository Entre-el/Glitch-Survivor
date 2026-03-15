using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
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
            InputManager.instance.isJoystickActive = true;
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
            Vector2 normalizedInput = clampedPosition / magnitudeRadius;
            InputManager.instance.SetMovementVector(normalizedInput);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handleRect.anchoredPosition = Vector2.zero;
        if(InputManager.instance != null) 
        {
            InputManager.instance.SetMovementVector(Vector2.zero);
            InputManager.instance.isJoystickActive = false; 
        }
    }
}
