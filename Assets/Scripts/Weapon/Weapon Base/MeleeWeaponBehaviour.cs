using UnityEngine;

public class MeleeWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float destroyAfterSeconds;
    protected float currentDamage;
    protected float currentCooldownDuration;
    protected int currentPierce;
    protected float currentSpeed;

    protected virtual void Awake()
    {
        currentDamage = weaponData.Damage;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
        currentSpeed = weaponData.Speed;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public float GetCurrentDamage()
    {
        return currentDamage * FindAnyObjectByType<PlayerStats>().CurrentMight;
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), enemy.transform.position);
        }
        else if(col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent<BreakableProps>(out BreakableProps prop))
            {
                prop.TakeDamage(GetCurrentDamage());
            }
        }
    }
}
