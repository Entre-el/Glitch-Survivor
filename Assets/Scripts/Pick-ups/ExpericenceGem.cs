using UnityEngine;

public class ExpericenceGem : PickUp, ICollectible
{
    public int experienceGranted;

    public void Collect()
    {
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            player.IncreaseExperience(experienceGranted);
            Destroy(gameObject); 
        }
    }
}
