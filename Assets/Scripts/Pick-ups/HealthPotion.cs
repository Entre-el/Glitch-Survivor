using UnityEngine;

public class HealthPotion : PickUp, ICollectible
{
    public int healthToRestore;

    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            player.RestoreHealth(healthToRestore);
            Destroy(gameObject);
        }
    }
}
