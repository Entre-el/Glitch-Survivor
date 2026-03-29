using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{
    public WeaponSO weaponData;
    protected Vector3 direction;
    public float destroyAfterSeconds;  
    protected float currentDamage;  
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;
    protected PlayerMovement pm;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public float GetCurrentDamage()
    {
        return currentDamage * FindAnyObjectByType<PlayerStats>().CurrentMight;
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir.normalized;
        float dirx = direction.x;
        float diry = direction.y;
        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = rotation.z + Mathf.Atan2(diry, dirx) * Mathf.Rad2Deg;
        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            ReducePierce();
            enemy.TakeDamage(GetCurrentDamage(), enemy.transform.position);
        }
        else if(col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent<BreakableProps>(out BreakableProps prop))
            {
                prop.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
        }
    }

    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
