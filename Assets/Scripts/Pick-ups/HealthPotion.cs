using UnityEngine;

public class HealthPotion : PickUp, ICollectible
{
    public int healthToRestore;
    [Header("Audio")]
    public AudioClip healthSFX;
    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            player.RestoreHealth(healthToRestore);
            AudioManager.Instance.PlaySFX(healthSFX,false);
            Destroy(gameObject);
        }
    }
}
