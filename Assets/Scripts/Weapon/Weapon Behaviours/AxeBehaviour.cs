using UnityEngine;
using System.Collections.Generic;

public class AxeBehaviour : MeleeWeaponBehaviour
{
    List<GameObject> markedEnemies = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
    }
    
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetCurrentDamage(), enemy.transform.position);
                markedEnemies.Add(col.gameObject);
            }
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps prop))
            {
                prop.TakeDamage(GetCurrentDamage());
                markedEnemies.Add(col.gameObject);
            }
        }
    }
}
