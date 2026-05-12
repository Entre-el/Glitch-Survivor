using UnityEngine;

public abstract class WeaponBrain : MonoBehaviour
{
    protected PlayerCore core;

    [Header("插槽管家")]
    public WeaponSlotManager SlotManager = new();

    [Header("基础配置")]
    public float FireRate = 2f;
    protected float lastFireTime;

    [Header("视觉与旋转绑定")]
    public Transform weaponPivot;
    public SpriteRenderer weaponSprite;
    public Transform emitter;

    // 缓存一个统一的瞄准方向
    protected Vector2 currentAimDirection;

    // 新增：记录武器挂载点相对于中心的绝对距离
    protected float defaultPivotX;
    float zDistance;

    public virtual void Initialize(PlayerCore core)
    {
        this.core = core;
        core.Locomotion.SetDashOverride(null, null);

        if (weaponPivot != null)
        {
            defaultPivotX = Mathf.Abs(weaponPivot.localPosition.x);
        }
        // 🌟 修复：获取相机到 Z=0 平面的绝对距离（通常是 10f）
        zDistance = Mathf.Abs(Camera.main.transform.position.z);
    }

    protected virtual void Update()
    {
        if (core == null || core.InputHandler == null)
            return;

        // 1. 统一获取并转换绝对瞄准方向
        CalculateAimDirection();

        // 2. 更新武器的视觉旋转
        UpdateWeaponRotation();

        // 3. 处理开火逻辑
        HandleFireInput();
    }

    // 🌟 核心：统一鼠标和手柄的输入差异
    private void CalculateAimDirection()
    {
        Vector2 aimIntent = core.InputHandler.AimIntent;
        Vector2 moveIntent = core.InputHandler.MoveIntent;

        if (aimIntent.magnitude > 0.1f)
        {
            if (core.InputHandler.IsUsingMouse && Camera.main != null)
            {
                Vector3 screenPos = new Vector3(aimIntent.x, aimIntent.y, zDistance);

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
                mouseWorldPos.z = 0f; // 强制将 Z 轴踩回平地，防止 2D 旋转错乱！

                Vector2 direction = (mouseWorldPos - transform.position);
                if (direction.sqrMagnitude > 0.01f)
                    currentAimDirection = direction.normalized;
            }
            else
            {
                currentAimDirection = aimIntent.normalized;
            }
        }
        else if (moveIntent.magnitude > 0.1f)
        {
            currentAimDirection = moveIntent.normalized;
        }
    }

    private void UpdateWeaponRotation()
    {
        if (weaponPivot == null || currentAimDirection == Vector2.zero)
            return;

        float angle = Mathf.Atan2(currentAimDirection.y, currentAimDirection.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        // 判断当前是不是在向左瞄准
        bool isAimingLeft = (angle > 90 || angle < -90);

        if (weaponSprite != null)
        {
            weaponSprite.flipY = isAimingLeft;
        }

        // 🌟 核心修复：动态调整挂载点位置！
        // 如果向左瞄准，把 X 坐标变成负数（挪到左肩）；如果向右，变成正数（挪到右肩）
        float targetX = isAimingLeft ? -defaultPivotX : defaultPivotX;

        // 保持 Y 和 Z 不变，只平移 X
        weaponPivot.localPosition = new Vector3(
            targetX,
            weaponPivot.localPosition.y,
            weaponPivot.localPosition.z
        );
    }

    protected virtual void HandleFireInput()
    {
        // 这里使用转换后的 currentAimDirection，而不是去读原始的 rawIntent！
        if (core.InputHandler.AimIntent.magnitude > 0.1f && Time.time >= lastFireTime + FireRate)
        {
            lastFireTime = Time.time;
            ExecuteFire(currentAimDirection);
        }
    }

    protected abstract void ExecuteFire(Vector2 direction);
}
