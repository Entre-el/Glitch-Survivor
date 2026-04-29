using UnityEngine;

public class WeaponEmitter : MonoBehaviour
{
    public ProjectileBase projectilePrefab;
    public Vector2 direction;
    public float projectileSpeed;
    public float projectileRange;
    public float projectileDamage;
    public float projectilePierce;

    public virtual ProjectileBase SpawnBullet()
    {
        Object newBullet = ObjectPoolManager.Instance.Get(projectilePrefab.gameObject);
        return newBullet as ProjectileBase;
    }
}
