using UnityEngine;

public class PickUp : MonoBehaviour
{
    protected bool isCollected = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (TryGetComponent<ICollectible>(out ICollectible collectible))
            {
                collectible.Collect();
                isCollected = true;
            }
        }
    }
}
