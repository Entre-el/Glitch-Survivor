using UnityEngine;

[CreateAssetMenu(menuName = "Stickers/Bubble Pop (Fade)")]
public class BubblePopStickerSO : StickerSO
{
    [Header("爆破特效 (可选)")]
    public GameObject popVfxPrefab;

    // 重写消失插槽的生命周期
    public override void OnFadeSlot(Vector3 fadePoint, CombatPayload payload)
    {
        // 打印日志，验证架构是否走通
        Debug.Log($"<color=#00FFFF>【晶片生效】泡泡在 {fadePoint} 破裂了！造成了 {payload.FinalDamage} 点范围伤害！</color>");

        // 如果配置了特效，从对象池拿出来播一下
        if (popVfxPrefab != null)
        {
            ObjectPoolManager.Instance.Get(popVfxPrefab, fadePoint, Quaternion.identity);
        }

        // TODO: 后续在这里调用 DamageResolver 进行 AoE 伤害结算
    }
}