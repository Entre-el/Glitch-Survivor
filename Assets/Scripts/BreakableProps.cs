using UnityEngine;

public class BreakableProps : MonoBehaviour
{
    public float health;
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Break();
        }
    }
    public void Break()
    {
        GetComponent<DropRateManagerr>().DropItem();
        Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
