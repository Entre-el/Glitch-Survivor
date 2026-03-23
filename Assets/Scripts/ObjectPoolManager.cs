using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private Dictionary<GameObject, IObjectPool<GameObject>> pools = new Dictionary<GameObject, IObjectPool<GameObject>>();
    private Dictionary<GameObject, Transform> poolRoots = new Dictionary<GameObject, Transform>();

    private void Awake()
    {
        if (Instance is null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public void ClearOldPools()
    {
        foreach (var pool in pools.Values)
        {
            pool.Clear();
        }
        pools.Clear();
        poolRoots.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void InitializePools(GameObject[] prefabsToPool)
    {
        ClearOldPools();

        foreach (var prefab in prefabsToPool)
        {
            // 防呆检测：防止策划拖了没挂组件的东西进来
            if (!prefab.TryGetComponent<PoolItem>(out var poolSettings))
            {
                Debug.LogError($"[对象池] 预制体 {prefab.name} 缺少 PoolItem 组件，无法建池！");
                continue; 
            }
            
            GameObject prefabToSpawn = prefab; // 规避闭包陷阱
            IObjectPool<GameObject> newPool = null; // 先声明，后赋值

            if (!poolRoots.ContainsKey(prefabToSpawn))
            {
                GameObject rootObj = new GameObject($"[Pool] {prefabToSpawn.name}");
                rootObj.transform.SetParent(this.transform);
                poolRoots.Add(prefabToSpawn, rootObj.transform);
            }

            newPool = new ObjectPool<GameObject>(
                createFunc: () => 
                {
                    // 这里是流水线：只需要老老实实从字典里拿出文件夹，然后生成物品
                    GameObject obj = Instantiate(prefabToSpawn, poolRoots[prefabToSpawn]);
                    
                    // 完美注入引用！
                    if (obj.TryGetComponent<PoolItem>(out var item))
                    {
                        item.SetPool(newPool);
                    }
                    return obj;
                },
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: poolSettings.defaultCapacity, 
                maxSize: poolSettings.maxSize
            );

            pools.Add(prefab, newPool);

            // 预热内存
            var prewarmList = new List<GameObject>(poolSettings.defaultCapacity);
            for (int i = 0; i < poolSettings.defaultCapacity; i++) prewarmList.Add(newPool.Get());
            foreach (var obj in prewarmList) newPool.Release(obj);
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position=default, Quaternion rotation=default)
    {
        if (pools.TryGetValue(prefab, out var pool))
        {
            GameObject obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }
        Debug.LogError($"[对象池] 未找到 {prefab.name} 的对象池！请检查关卡配置！");
        return null;
    }
}
