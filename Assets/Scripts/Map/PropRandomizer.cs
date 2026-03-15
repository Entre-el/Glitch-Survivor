using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    [System.Serializable]
    public class Prop
    {
        public string tag;
        public float spawnChance;
    }

    [Header("Spawn Points")]
    public Transform[] propSpawnPoints;

    [Header("Props")]
    public List<Prop> props; 

    [Header("Density")]
    [Range(0f, 1f)]
    public float ChunkDensity = 0.4f;

    private class SpawnedPropData
    {
        public GameObject obj;
        public string tag;
    }
    private List<SpawnedPropData> activeProps = new List<SpawnedPropData>();

    void OnEnable() 
    {
        SpawnProps();
    }

    void OnDisable()
    {
        ClearProps();
    }

    void SpawnProps()
    {
        if (propSpawnPoints == null || propSpawnPoints.Length == 0)
        {
            Debug.LogWarning("PropRandomizer: propSpawnPoints not set!");
            return;
        }

        if (props == null || props.Count == 0)
        {
            Debug.LogWarning("PropRandomizer: props list not set!");
            return;
        }

        foreach(Transform sp in propSpawnPoints)
        {
            if (Random.value > ChunkDensity) continue;
            Prop selectedProp = null;
            float allChance = 0;
            foreach(Prop p in props)
            {
                allChance += p.spawnChance;
            }
            float randomValue = Random.value * allChance;
            foreach(Prop p in props)
            {
                if (randomValue <= p.spawnChance)
                {
                    selectedProp = p;
                    break;
                }
                randomValue -= p.spawnChance;
            }

            if (selectedProp == null)
            {
                Debug.LogWarning("PropRandomizer: Random selection failed.");
                continue;
            }

            if (ObjectPoolManager.Instance == null)
            {
                Debug.LogError("PropRandomizer: ObjectPoolManager.Instance is null!");
                continue;
            }

            GameObject prop = ObjectPoolManager.Instance.Get(selectedProp.tag);
            
            if (prop != null)
            {
                float offSetX = Random.Range(-0.5f, 0.5f);
                float offSetY = Random.Range(-0.5f, 0.5f);
                prop.transform.SetParent(sp);
                prop.transform.position = new Vector3(offSetX, offSetY, 0);
                activeProps.Add(new SpawnedPropData { obj = prop, tag = selectedProp.tag });
            }
        }
    }

    void ClearProps()
    {
        if (ObjectPoolManager.Instance == null)
        {
            activeProps.Clear();
            return;
        }

        foreach(var data in activeProps)
        {
            data.obj.transform.SetParent(null);
            data.obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            ObjectPoolManager.Instance.Release(data.tag, data.obj);
        }
        
        activeProps.Clear();
    }
}
