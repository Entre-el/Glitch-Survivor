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

    [Header("手感调校")]
    [Tooltip("满速宽容区：0.8表示手指拉到80%的距离就输出满速")]
    [Range(0.5f, 1f)]
    public float fullSpeedThreshold = 0.8f; 

    private Canvas parentCanvas;
    private float magnitudeRadius;

    private void Start()
    {
        // 计算大圆圈物理半径的一半
        magnitudeRadius = backgroundRect.sizeDelta.x / 2f;
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(InputManager.instance != null)
        {
            if (joystickType == JoystickType.Movement)
                InputManager.instance.isMoveJoystickActive = true;
            else
                InputManager.instance.isAimJoystickActive = true;
        }
        
        // 按下的瞬间立刻执行一次拖拽逻辑，实现“点哪里小圆圈瞬间飞过去”
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
            // 1. UI 视觉层：限制小圆圈的物理移动范围不超过大圆圈
            Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, magnitudeRadius);
            handleRect.anchoredPosition = clampedPosition;
            
            // 2. 逻辑输出层：计算带宽容度的向量
            Vector2 normalizedInput = Vector2.zero;
            if (magnitudeRadius > 0.001f) 
            {
                // 计算当前拖拽的绝对距离
                float currentDistance = clampedPosition.magnitude;
                
                // 计算真正的“有效满速半径”
                float actualActiveRadius = magnitudeRadius * fullSpeedThreshold;
                
                // 计算当前的推力强度，并强行限制在 0 到 1 之间
                // (如果超过 activeRadius，这里会自动被 Clamp01 截断为 1)
                float inputMagnitude = Mathf.Clamp01(currentDistance / actualActiveRadius);

                // 如果有推力，输出最终的方向向量 * 强度
                if (currentDistance > 0.001f)
                {
                    // (clampedPosition / currentDistance) 就是极其精准的纯方向向量
                    normalizedInput = (clampedPosition / currentDistance) * inputMagnitude;
                }
            }
            
            // 3. 分发给大管家
            if (joystickType == JoystickType.Movement)
                InputManager.instance.SetMovementVector(normalizedInput);
            else
                InputManager.instance.SetAimVector(normalizedInput);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 抬手时小圆圈回正
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