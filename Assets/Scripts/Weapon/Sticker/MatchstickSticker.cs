using UnityEngine;
[CreateAssetMenu(menuName = "Stickers/火柴贴纸")]
public class MatchstickSticker : StickerSO
{
    [Header("火柴专属数值")]
    public float speedMultiplier = 1.5f;
    public float burnDamage = 5f;
    public GameObject fireAreaPrefab; // 消失时生成的火海预制体

}