using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("玩家数据")]
    public List<StickerSO> ownedStickers = new();

    // 🌟 引用：指向玩家当前手里拿的 WeaponBrain 里的那个 SlotManager
    public WeaponSlotManager currentWeaponSlotManager;

    private void Awake()
    {
        Instance = this;
    }

    // 🌟 核心桥梁：当玩家生成/切换武器时，调用这个方法！
    public void BindActiveWeapon(WeaponBrain activeBrain)
    {
        if (activeBrain != null)
        {
            currentWeaponSlotManager = activeBrain.SlotManager;
            //Debug.Log($"🎒 背包系统已连接到武器：{activeBrain.gameObject.name}");
        }
    }

    public List<StickerSO> GetUnequippedStickers()
    {
        if (currentWeaponSlotManager == null)
            return new List<StickerSO>(ownedStickers);

        List<StickerSO> unequipped = new List<StickerSO>();

        // 🌟 1. 查账本：统计背包里每种贴纸的总数量
        Dictionary<StickerSO, int> availableCounts = new Dictionary<StickerSO, int>();
        foreach (var sticker in ownedStickers)
        {
            if (!availableCounts.ContainsKey(sticker))
            {
                availableCounts[sticker] = 0;
            }
            availableCounts[sticker]++;
        }

        // 🌟 2. 扣除余额：把武器上已经装备的贴纸数量扣掉
        if (
            currentWeaponSlotManager.FireSticker != null
            && availableCounts.ContainsKey(currentWeaponSlotManager.FireSticker)
        )
            availableCounts[currentWeaponSlotManager.FireSticker]--;

        if (currentWeaponSlotManager.SubContexts.Count > 0)
        {
            var ctx = currentWeaponSlotManager.SubContexts[0];

            if (ctx.PierceSticker != null && availableCounts.ContainsKey(ctx.PierceSticker))
                availableCounts[ctx.PierceSticker]--;

            if (ctx.CritSticker != null && availableCounts.ContainsKey(ctx.CritSticker))
                availableCounts[ctx.CritSticker]--;

            if (ctx.FadeSticker != null && availableCounts.ContainsKey(ctx.FadeSticker))
                availableCounts[ctx.FadeSticker]--;
        }

        // 🌟 3. 发放散落物资：把账本上剩下的贴纸重新变成列表，交给 UI 去散落
        foreach (var kvp in availableCounts)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                unequipped.Add(kvp.Key);
            }
        }

        return unequipped;
    }

    // 🌟 核心接口：UI 拖拽成功后，尝试装备贴纸
    public bool TryEquipSticker(StickerSO sticker, StickerSlotType slotType)
    {
        if (currentWeaponSlotManager == null)
            return false;

        // 🛡️ 校验 1：贴纸类型是否匹配槽位？
        if (sticker.compatibleSlot != StickerSlotType.Any && sticker.compatibleSlot != slotType)
        {
            Debug.LogWarning(
                $"⚠️ 装配失败：贴纸 [{sticker.stickerName}] 不兼容 [{slotType}] 槽位！"
            );
            return false;
        }

        // 🛡️ 校验 2：安全防越界（确保武器的 SubContexts 至少有 1 个元素）
        if (currentWeaponSlotManager.SubContexts == null)
            currentWeaponSlotManager.SubContexts = new List<SubWeaponContext>();
        if (currentWeaponSlotManager.SubContexts.Count == 0)
            currentWeaponSlotManager.SubContexts.Add(new SubWeaponContext());

        // 🌟 真实修改数据：直接改武器肚子里的那个管家！
        switch (slotType)
        {
            case StickerSlotType.Fire:
                currentWeaponSlotManager.FireSticker = sticker;
                break;
            case StickerSlotType.Pierce:
                currentWeaponSlotManager.SubContexts[0].PierceSticker = sticker;
                break;
            case StickerSlotType.Crit:
                currentWeaponSlotManager.SubContexts[0].CritSticker = sticker;
                break;
            case StickerSlotType.Fade:
                currentWeaponSlotManager.SubContexts[0].FadeSticker = sticker;
                break;
        }

        //Debug.Log($"✅ 成功将 [{sticker.stickerName}] 装备到了 [{slotType}] 槽位！");
        return true;
    }

    // 🌟 核心接口：UI 点击槽位后，卸下贴纸
    public void UnequipSticker(StickerSlotType slotType)
    {
        if (currentWeaponSlotManager == null)
            return;

        // 直接将对应槽位的数据清空（置为 null）
        switch (slotType)
        {
            case StickerSlotType.Fire:
                currentWeaponSlotManager.FireSticker = null;
                break;
            case StickerSlotType.Pierce:
                if (currentWeaponSlotManager.SubContexts.Count > 0)
                    currentWeaponSlotManager.SubContexts[0].PierceSticker = null;
                break;
            case StickerSlotType.Crit:
                if (currentWeaponSlotManager.SubContexts.Count > 0)
                    currentWeaponSlotManager.SubContexts[0].CritSticker = null;
                break;
            case StickerSlotType.Fade:
                if (currentWeaponSlotManager.SubContexts.Count > 0)
                    currentWeaponSlotManager.SubContexts[0].FadeSticker = null;
                break;
        }

        //Debug.Log($"⬇️ 成功卸下了 [{slotType}] 槽位的贴纸！");
    }
}
