using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisuals : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerCore core;

    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int isDashingHash = Animator.StringToHash("IsDashing");
    private float zDistance;


    public void Initialize(PlayerCore core)
    {
        TryGetComponent<Animator>(out animator);
        TryGetComponent<SpriteRenderer>(out spriteRenderer);
        this.core = core;

        EventCenter.AddListener(EventDefine.OnPlayerDashed, OnPlayerDashed);
        EventCenter.AddListener(EventDefine.OnPlayerDashEnd, OnPlayerDashEnd);
        zDistance = Mathf.Abs(Camera.main.transform.position.z);

    }
private void Update()
    {
        if (core == null || core.InputHandler == null) return;

        Vector2 moveIntent = core.InputHandler.MoveIntent;
        animator.SetFloat(speedHash, moveIntent.magnitude); 

        Vector2 aimIntent = core.InputHandler.AimIntent;

        if (aimIntent.magnitude > 0.1f)
        {
            if (core.InputHandler.IsUsingMouse)
            {
                if (Camera.main != null)
                {
                    // 🌟 修复：同样使用距离作为深度
                    Vector3 screenPos = new(aimIntent.x, aimIntent.y, zDistance);
                    
                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
                    spriteRenderer.flipX = mouseWorldPos.x < transform.position.x;
                }
            }
            else
            {
                spriteRenderer.flipX = aimIntent.x < 0;
            }
        }
        else if (moveIntent.x != 0)
        {
            spriteRenderer.flipX = moveIntent.x < 0;
        }
    }

    private void OnPlayerDashed()
    {
        animator.SetBool(isDashingHash, true);
    }

    private void OnPlayerDashEnd()
    {
        animator.SetBool(isDashingHash, false);
    }

    public void TriggerAttackAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }
    
    public void PlayVFX(GameObject vfxPrefab, Vector3 offset)
    {
        ObjectPoolManager.Instance.Get(vfxPrefab, transform.position + offset, Quaternion.identity);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.OnPlayerDashed, OnPlayerDashed);
        EventCenter.RemoveListener(EventDefine.OnPlayerDashEnd, OnPlayerDashEnd);
    }
}