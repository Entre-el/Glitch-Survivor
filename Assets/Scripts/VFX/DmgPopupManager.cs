using System;
using UnityEngine;

public class DmgPopupManager : MonoBehaviour
{
    public DmgText dmgPopupPrefab;
    public Canvas DmgCanvas; // 确保这个 Canvas 的 Render Mode 是 World Space，并且正确设置了 Sorting Layer

    void Start()
    {
        EventCenter.AddListener<DmgMessage>(EventDefine.OnDamagePopup, ShowDamagePopup);
    }

    private void ShowDamagePopup(DmgMessage message)
    {
        // 🌟 修复：作为局部变量，防止并发覆盖。并把名字从容易混淆的 prefab 改为 popup
        DmgText popup = ObjectPoolManager.Instance.Get<DmgText>(dmgPopupPrefab.gameObject);
        popup.transform.SetParent(DmgCanvas.transform, false); // 设置父对象为 Canvas，保持局部缩放不变
        popup.SetText(message.amount.ToString());
        popup.transform.position = message.position;
        popup.SetCritical(message.isCritical);

        // TODO: 在这里触发 popup 的上浮动画，并在动画结束时回收它
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<DmgMessage>(EventDefine.OnDamagePopup, ShowDamagePopup);
    }
}
