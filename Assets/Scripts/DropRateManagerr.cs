using System.Collections.Generic;
using UnityEngine;

public class DropRateManagerr : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate; // Percentage chance to drop (0-100)
    }
    public List<Drops> drops;
    public void DropItem()
    {
        List<Drops> possibleDrops  = new List<Drops>();
        float randomValue = Random.Range(0f, 100f);
        foreach (Drops drop in drops)
        {
            if (randomValue <= drop.dropRate)
            {
                possibleDrops.Add(drop);
            }
        }
        if (possibleDrops.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleDrops.Count);
            Instantiate(possibleDrops[randomIndex].itemPrefab, transform.position, Quaternion.identity);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
