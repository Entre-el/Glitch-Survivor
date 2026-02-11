using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{
    protected Vector3 direction;
    public float destroyAfterSeconds;  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir; 
        float dirx = direction.x;
        float diry = direction.y;
        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = rotation.z + Mathf.Atan2(diry, dirx) * Mathf.Rad2Deg;
        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
