using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerInventoryPanel : BasePanel
{
    [Header("散落区域设置")]
    public RectTransform scatterZone; // 底部散落区的空UI节点
    public float minScatterDistance = 120f; // 贴纸之间的最小防重叠像素距离

    [Header("预制体与数据")]
    public GameObject stickerPrefab; // 贴纸UI的预制体
    [Header("武器槽位引用")]
    public WeaponSlotUI fireSlot;
    public WeaponSlotUI pierceSlot;
    public WeaponSlotUI critSlot;
    public WeaponSlotUI fadeSlot;
    // 记录已经生成的散落坐标，用于防重叠判定
    private List<Vector2> occupiedPositions = new(4);

    public override void OnShow()
    {
        base.OnShow();
        occupiedPositions.Clear();
        // 1. 获取底层的数据管家
        WeaponSlotManager weaponData = InventoryManager.Instance.currentWeaponSlotManager;

        // 2. 强制同步 UI 槽位的视觉表现
        if (weaponData != null)
        {
            fireSlot.UpdateVisual(weaponData.FireSticker);
            
            if (weaponData.SubContexts.Count > 0)
            {
                pierceSlot.UpdateVisual(weaponData.SubContexts[0].PierceSticker);
                // ... 同步其他槽位
            }
        }
        // 假设从 InventoryManager 获取未安装的贴纸列表
        List<StickerSO> unequippedStickers = InventoryManager.Instance.GetUnequippedStickers();
        
        foreach (var stickerData in unequippedStickers)
        {
            SpawnScatteredSticker(stickerData);
        }
    }

    private void SpawnScatteredSticker(StickerSO data)
    {
        // 实例化贴纸
        GameObject stickerObj = Instantiate(stickerPrefab, scatterZone);
        DraggableStickerUI dragLogic = stickerObj.GetComponent<DraggableStickerUI>();
        
        // 寻找一个不重叠的随机本地坐标
        Vector2 targetPos = GetNonOverlappingPosition();
        occupiedPositions.Add(targetPos);
        
        // 生成一个随机旋转角度 (-30 到 30度)
        float randomAngle = Random.Range(-30f, 30f);
        
        // 注入数据并让贴纸自己飞过去
        dragLogic.Initialize(data, targetPos, randomAngle);
    }

    // 🌟 核心：随机散落且防重叠的数学算法
    private Vector2 GetNonOverlappingPosition()
    {
        int maxAttempts = 30; // 防止死循环
        float width = scatterZone.rect.width / 2f - 60f; // 减去边缘缓冲
        float height = scatterZone.rect.height / 2f - 60f;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = new Vector2(Random.Range(-width, width), Random.Range(-height, height));
            bool isOverlapping = false;

            // 检查与现有贴纸的距离
            foreach (var pos in occupiedPositions)
            {
                if (Vector2.Distance(randomPos, pos) < minScatterDistance)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (!isOverlapping) return randomPos; // 找到合适的位置！
        }
        
        // 如果试了30次都没找到（东西太多了），强制返回一个坐标
        return Vector2.zero; 
    }
}