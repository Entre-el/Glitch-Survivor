using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class EnemyVisuals : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_1 = new(0.1f);

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine flashCoroutine;
    private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

    public void Initialize(EnemyCore core)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void PlayHitEffect()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 1. 获取当前渲染器挂载的块数据
        spriteRenderer.GetPropertyBlock(mpb);

        // 2. 写入覆盖数据 (全白，强度 1)
        mpb.SetColor(FlashColorID, Color.white);
        mpb.SetFloat(FlashAmountID, 1f);

        // 3. 提交至 GPU 渲染队列
        spriteRenderer.SetPropertyBlock(mpb);

        yield return _waitForSeconds0_1;

        // 恢复正常状态 (强度设为 0)
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat(FlashAmountID, 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
