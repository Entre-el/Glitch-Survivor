using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; 

    [Header("玩家数据")]
    public List<StickerSO> ownedStickers = new List<StickerSO>();
    
    // 🌟 修改：不要在这里 new，而是作为一个“引用”
    // 它指向的是玩家当前手里拿的 WeaponBrain 里的那个 SlotManager
    public WeaponSlotManager currentWeaponSlotManager; 

    private void Awake() { Instance = this; }

    // 🌟 核心桥梁：当玩家生成/切换武器时，调用这个方法！
    public void BindActiveWeapon(WeaponBrain activeBrain)
    {
        if (activeBrain != null)
        {
            // 顺藤摸瓜，拿到这把武器的插槽管家
            currentWeaponSlotManager = activeBrain.SlotManager;
            Debug.Log($"🎒 背包系统已连接到武器：{activeBrain.gameObject.name}");
        }
    }

    public List<StickerSO> GetUnequippedStickers()
    {
        // 如果手里没武器，或者还没绑定，那就所有的贴纸都是未装备的
        if (currentWeaponSlotManager == null) return new List<StickerSO>(ownedStickers);

        List<StickerSO> unequipped = new List<StickerSO>();
        HashSet<StickerSO> equippedSet = new HashSet<StickerSO>();
        
        // 🌟 使用引用的 currentWeaponSlotManager 来判断
        if (currentWeaponSlotManager.FireSticker != null) equippedSet.Add(currentWeaponSlotManager.FireSticker);
        
        if (currentWeaponSlotManager.SubContexts.Count > 0)
        {
            var ctx = currentWeaponSlotManager.SubContexts[0];
            if (ctx.PierceSticker != null) equippedSet.Add(ctx.PierceSticker);
            // ... 其他槽位
        }

        foreach (var sticker in ownedStickers)
        {
            if (!equippedSet.Contains(sticker)) unequipped.Add(sticker);
        }

        return unequipped;
    }

    public bool TryEquipSticker(StickerSO sticker, StickerSlotType slotType)
    {
        if (currentWeaponSlotManager == null) return false;

        // 校验逻辑... (省略)

        // 🌟 真实修改数据：直接改武器肚子里的那个管家！
        switch (slotType)
        {
            case StickerSlotType.Fire: currentWeaponSlotManager.FireSticker = sticker; break;
            case StickerSlotType.Pierce: currentWeaponSlotManager.SubContexts[0].PierceSticker = sticker; break;
            // ... 其他槽位
        }
        return true;
    }
}