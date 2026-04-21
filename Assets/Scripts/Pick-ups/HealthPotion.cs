using UnityEngine;

public class HealthPotion : PickUp, ICollectible
{
    public int healthToRestore;
    [Header("Audio")]
    public AudioClip healthSFX;
    public void Collect()
    {
        PlayerCore playerCore = FindAnyObjectByType<PlayerCore>();
        if (playerCore != null)
        {
            playerCore.Health.Heal(healthToRestore);
            AudioManager.Instance.PlaySFX(healthSFX,false);
            Destroy(gameObject);
        }
    }
}
