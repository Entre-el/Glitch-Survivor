using UnityEngine;

public class GarlicController : WeaponController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Attack() 
    {
        base.Attack();
        GameObject spawnedGarlic = Instantiate(prefab);
        spawnedGarlic.transform.position = transform.position;
        spawnedGarlic.transform.parent = transform;
    }
}