using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public GameObject[] propSpanwPoints;
    public GameObject[] propPrefabs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnProps();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnProps()
    {
        foreach(GameObject sp in propSpanwPoints)
        {
            int rand = Random.Range(0, propPrefabs.Length);
            GameObject prop = Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity);
            prop.transform.SetParent(sp.transform);
        }
    }
}
