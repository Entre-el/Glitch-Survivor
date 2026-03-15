using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public string tag;
        public GameObject prefab;
        public int defaultCapacity = 50;
        public int maxSize = 200;
    }

    [Header("Pool Configs")]
    public List<PoolConfig> poolConfigs;

    private Dictionary<string, IObjectPool<GameObject>> pools;
    private Dictionary<string, GameObject> prefabLookup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        pools = new Dictionary<string, IObjectPool<GameObject>>();
        prefabLookup = new Dictionary<string, GameObject>();

        foreach (var config in poolConfigs)
        {
            prefabLookup[config.tag] = config.prefab;
            string capturedTag = config.tag;

            IObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefabLookup[capturedTag], transform),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );

            pools.Add(config.tag, newPool);

            // Pre-warm the pool
            var prewarmList = new List<GameObject>();
            for (int i = 0; i < config.defaultCapacity; i++)
            {
                prewarmList.Add(newPool.Get());
            }
            foreach (var obj in prewarmList)
            {
                newPool.Release(obj);
            }
        }
    }

    public GameObject Get(string tag)
    {
        return pools.ContainsKey(tag) ? pools[tag].Get() : null;
    }

    public void Release(string tag, GameObject obj)
    {
        if (pools.ContainsKey(tag)) pools[tag].Release(obj);
    }
}
