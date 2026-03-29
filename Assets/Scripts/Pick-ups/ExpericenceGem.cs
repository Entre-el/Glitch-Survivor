using UnityEngine;

public class ExpericenceGem : PickUp, ICollectible
{
    public int experienceGranted;
    [Header("Audio")]
    public AudioClip experienceSFX;
    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            player.IncreaseExperience(experienceGranted);
            AudioManager.Instance.PlayPickupSFX(experienceSFX);
            Destroy(gameObject); 
        }
    }
}
