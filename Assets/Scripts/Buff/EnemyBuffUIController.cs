using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffUIController : MonoBehaviour
{
    [Header("UI 设置")]
    [SerializeField]
    private BuffUIItem buffItemPrefab;

    [SerializeField]
    private int maxDisplayCount = 3;

    [SerializeField]
    private float iconSpacing = 0.5f;

    private readonly List<BuffUIItem> uiItems = new();

    // 标记是否已经初始化过
    private bool isInitialized = false;

    public void Initialize()
    {
        if (isInitialized)
            return; // 防止重复生成

        for (int i = 0; i < maxDisplayCount; i++)
        {
            BuffUIItem item = Instantiate(buffItemPrefab, transform);
            item.SetActive(false);
            uiItems.Add(item);
        }
        isInitialized = true;
    }

    public void UpdateBuffDisplay(List<BaseEnemyBuff> activeBuffs)
    {
        if (!isInitialized)
        {
            Initialize();
        }

        int displayCount = Mathf.Min(activeBuffs.Count, maxDisplayCount);

        // 计算居中偏移量
        float startX = -(displayCount - 1) * iconSpacing / 2f;

        for (int i = 0; i < maxDisplayCount; i++)
        {
            if (i < displayCount)
            {
                BaseEnemyBuff buff = activeBuffs[i];

                uiItems[i].Setup(buff.buffData.buffIcon, buff.stackCount);
                uiItems[i].transform.localPosition = new Vector3(startX + (i * iconSpacing), 0, 0);
                uiItems[i].SetActive(true);
            }
            else
            {
                uiItems[i].SetActive(false);
            }
        }
    }

    public void ClearDisplay()
    {
        // 同样加入防御
        if (!isInitialized)
            return;

        foreach (var item in uiItems)
        {
            item.SetActive(false);
        }
    }
}
