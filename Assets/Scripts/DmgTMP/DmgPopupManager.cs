using UnityEngine;

public class DmgPopupManager : MonoBehaviour
{
    // 替换为最新的表现层类名
    public DamagePopupUI dmgPopupPrefab;
    public Transform popupContainer;

    [Header("性能限制")]
    public int maxPopupsOnScreen = 40; // 同屏最多 40 个飘字

    // 静态计数器：全局追踪当前激活的飘字数量
    public static int CurrentActivePopups = 0;

    void Start()
    {
        // 注册全局伤害监听
        EventCenter.AddListener<DmgMessage>(EventDefine.OnDamagePopup, ShowDamagePopup);
    }

    private void ShowDamagePopup(DmgMessage message)
    {
        if (CurrentActivePopups >= maxPopupsOnScreen)
        {
            return;
        }
        // 从对象池获取实例
        DamagePopupUI popup = ObjectPoolManager.Instance.Get<DamagePopupUI>(
            dmgPopupPrefab.gameObject
        );
        if (popup != null)
        {
            CurrentActivePopups++;
        }

        // 维持UI层级与世界坐标
        popup.transform.SetParent(popupContainer, false);
        popup.transform.position = message.position;

        // 🌟 核心修改：Manager 只负责转发数据载荷，将渲染职责完全下放给 View 层
        popup.Setup(message);
    }

    void OnDestroy()
    {
        // 注销监听防内存泄漏
        EventCenter.RemoveListener<DmgMessage>(EventDefine.OnDamagePopup, ShowDamagePopup);
    }
}
