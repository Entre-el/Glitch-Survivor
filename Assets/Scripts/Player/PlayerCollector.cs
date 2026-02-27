using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;
    void  Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }
    void Update()
    {
        playerCollector.radius = player.currentMagnet;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent<ICollectible>(out ICollectible collectible))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 forceDirection = (transform.position - col.transform.position).normalized;
                rb.AddForce(forceDirection * pullSpeed);
            }
        }
    }
}
