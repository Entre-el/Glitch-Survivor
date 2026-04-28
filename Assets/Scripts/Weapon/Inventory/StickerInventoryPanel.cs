using System.Collections.Generic;
using UnityEngine;

public class StickerInventoryPanel : BasePanel
{
    [Header("区域与预制体设置")]
    public RectTransform scatterZone; // 底部散落区的空UI节点
    public RectTransform stickerZone; // 实际挂载贴纸的节点
    public GameObject stickerPrefab; // 贴纸UI的预制体

    [Header("武器槽位引用")]
    public WeaponSlotUI fireSlot;
    public WeaponSlotUI pierceSlot;
    public WeaponSlotUI critSlot;
    public WeaponSlotUI fadeSlot;

    [Header("物理参数：全局散落 (从天而降)")]
    [Tooltip("每次生成贴纸时X轴增加的偏移量，防止完全重叠")]
    public float spawnOffsetXStep = 80f;
    public Vector2 scatterVelocityX = new(-500f, 500f);
    public Vector2 scatterVelocityY = new(-200f, -800f);
    public Vector2 scatterAngularVelocity = new(-300f, 300f);

    [Header("物理参数：槽位拆卸 (精准空投)")]
    public Vector2 ejectVelocityX = new(-300f, 300f);
    public Vector2 ejectVelocityY = new(300f, 800f);
    public Vector2 ejectAngularVelocity = new(-300f, 300f);

    // 用于记录上一次生成的X坐标，实现依次排开的效果
    private float currentSpawnX = 0f;

    public override void OnShow()
    {
        base.OnShow();

        // 1. 清理旧的散落物并激活区域
        foreach (Transform child in stickerZone)
        {
            Destroy(child.gameObject);
        }
        scatterZone.gameObject.SetActive(true);

        // 2. 获取数据并同步 UI 槽位
        WeaponSlotManager weaponData = InventoryManager.Instance.currentWeaponSlotManager;
        if (weaponData != null)
        {
            fireSlot.UpdateVisual(weaponData.FireSticker);

            if (weaponData.SubContexts != null && weaponData.SubContexts.Count > 0)
            {
                var subCtx = weaponData.SubContexts[0];
                pierceSlot.UpdateVisual(subCtx.PierceSticker);
                critSlot.UpdateVisual(subCtx.CritSticker);
                fadeSlot.UpdateVisual(subCtx.FadeSticker);
            }
        }
        else
        {
            Debug.LogWarning("打开背包时，没有检测到绑定的武器！");
        }

        // 3. 获取未安装的贴纸并从顶部散落
        List<StickerSO> unequippedStickers = InventoryManager.Instance.GetUnequippedStickers();

        // 🌟 初始化生成起点 (从左侧开始)
        float halfWidth = stickerZone.rect.width / 2f - 50f;
        currentSpawnX = -halfWidth;

        foreach (var stickerData in unequippedStickers)
        {
            SpawnScatteredSticker(stickerData, halfWidth);
        }
    }

    public override void OnHide()
    {
        base.OnHide();
        scatterZone.gameObject.SetActive(false);
    }

    // 🌟 从天而降：用于打开背包时的全局散落
    private void SpawnScatteredSticker(StickerSO data, float halfWidth)
    {
        Rigidbody2D rb = CreateStickerBase(data, out GameObject stickerObj);

        // 🌟 核心排版逻辑：叠加 X 轴偏移量
        currentSpawnX += spawnOffsetXStep;

        // 如果超出了右边界，折返回左侧（稍微加点随机防死板）
        if (currentSpawnX > halfWidth)
        {
            currentSpawnX = -halfWidth + Random.Range(0f, 40f);
        }

        float topY = stickerZone.rect.height / 4f;

        // 应用坐标
        stickerObj.transform.localPosition = new Vector3(currentSpawnX, topY, 0f);

        // 应用面板中配置的散落物理力
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                Random.Range(scatterVelocityX.x, scatterVelocityX.y),
                Random.Range(scatterVelocityY.x, scatterVelocityY.y)
            );
            rb.angularVelocity = Random.Range(scatterAngularVelocity.x, scatterAngularVelocity.y);
        }
    }

    // 🌟 精准空投：用于从槽位上扣下贴纸
    public void SpawnStickerAtMouse(StickerSO data, Vector2 screenPosition, Camera uiCamera = null)
    {
        Rigidbody2D rb = CreateStickerBase(data, out GameObject stickerObj);

        // 屏幕坐标精准转换为散落区的本地坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            stickerZone,
            screenPosition,
            uiCamera,
            out Vector2 localPoint
        );

        stickerObj.transform.localPosition = (Vector3)localPoint;

        // 应用面板中配置的弹射物理力
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                Random.Range(ejectVelocityX.x, ejectVelocityX.y),
                Random.Range(ejectVelocityY.x, ejectVelocityY.y)
            );
            rb.angularVelocity = Random.Range(ejectAngularVelocity.x, ejectAngularVelocity.y);
        }
    }

    // 🛠️ 核心工具方法：提取重复的实例化和初始化逻辑
    private Rigidbody2D CreateStickerBase(StickerSO data, out GameObject stickerObj)
    {
        stickerObj = Instantiate(stickerPrefab, stickerZone);
        stickerObj.transform.localScale = Vector3.one;
        stickerObj.SetActive(true);

        if (stickerObj.TryGetComponent<DraggableStickerUI>(out var dragLogic))
        {
            dragLogic.Initialize(data);
        }

        if (scatterZone.TryGetComponent<PhysicsSimulationZone>(out var zone))
        {
            zone.MarkCacheDirty();
        }

        return stickerObj.GetComponent<Rigidbody2D>();
    }
}
