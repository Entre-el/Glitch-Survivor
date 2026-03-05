using UnityEngine;
using System.Collections.Generic;

public class GarlicBehaviour : MeleeWeaponBehaviour
{
    List<GameObject> markedEnemise = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }
    
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemise.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetCurrentDamage());
                markedEnemise.Add(col.gameObject);
            }
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent<BreakableProps>(out BreakableProps prop))
            {
                prop.TakeDamage(GetCurrentDamage());
                markedEnemise.Add(col.gameObject);
            }
        }
    }
}
