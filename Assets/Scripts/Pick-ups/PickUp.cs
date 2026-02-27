using UnityEngine;

public class PickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (TryGetComponent<ICollectible>(out ICollectible collectible))
            {
                collectible.Collect();
            }
        }
    }
}
