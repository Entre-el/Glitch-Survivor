using UnityEngine;

public class KnifeBehaviour : MonoBehaviour
{
    public void DirectionChecker(Vector3 dir)
    {
        Vector3 direction = dir.normalized;
        float dirx = direction.x;
        float diry = direction.y;
        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = rotation.z + Mathf.Atan2(diry, dirx) * Mathf.Rad2Deg;
        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
