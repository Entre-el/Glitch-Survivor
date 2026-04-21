using UnityEngine;
// SO 锚点定义
[CreateAssetMenu(menuName = "Anchors/Transform Anchor")]
public class TransformAnchorSO : ScriptableObject 
{
    public Transform Value; // 锚点数据
}