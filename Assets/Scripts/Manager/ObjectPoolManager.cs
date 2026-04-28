using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    
    // Key: 预制体 (Prefab)
    private Dictionary<GameObject, IObjectPool<GameObject>> pools = new Dictionary<GameObject, IObjectPool<GameObject>>();
    private Dictionary<GameObject, Transform> poolRoots = new Dictionary<GameObject, Transform>();

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void ClearOldPools()
    {
        foreach (var pool in pools.Values) pool.Clear();
        pools.Clear();
        poolRoots.Clear();
        foreach (Transform child in transform) Destroy(child.gameObject);
    }

    // 1. 保留原本的批量初始化，但内部改为调用单个注册
    public void InitializePools(GameObject[] prefabsToPool)
    {
        ClearOldPools();
        foreach (var prefab in prefabsToPool)
        {
            RegisterPool(prefab);
        }
    }

    // 2. 新增：单体按需注册函数 (核心外包逻辑)
    public void RegisterPool(GameObject prefab)
    {
        // 防御：如果已经建过这个池子了，直接跳过，防止重复创建
        if (pools.ContainsKey(prefab)) return;

        if (!prefab.TryGetComponent<PoolItem>(out var poolSettings))
        {
            Debug.LogError($"[对象池] 预制体 {prefab.name} 缺少 PoolItem 组件，无法建池！");
            return; 
        }
        
        GameObject prefabToSpawn = prefab; 
        IObjectPool<GameObject> newPool = null; 

        if (!poolRoots.ContainsKey(prefabToSpawn))
        {
            GameObject rootObj = new GameObject($"[Pool] {prefabToSpawn.name}");
            rootObj.transform.SetParent(this.transform);
            poolRoots.Add(prefabToSpawn, rootObj.transform);
        }

        newPool = new ObjectPool<GameObject>(
            createFunc: () => 
            {
                GameObject obj = Instantiate(prefabToSpawn, poolRoots[prefabToSpawn]);
                // 完美注入！这里是对象池架构的精髓
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

    // 3. 增强版的 Get：加入“按需建池 (Lazy Init)”机制
    public GameObject Get(GameObject prefab, Vector3 position = default, Quaternion rotation = default)
    {
        // 如果发现有人想要东西，但池子还没建，帮他当场建一个！(极致的容错率)
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogWarning($"[对象池] 试图获取未注册的预制体 {prefab.name}，正在自动为您按需建池...");
            RegisterPool(prefab);
        }

        // 此时池子必定存在（除非预制体没挂 PoolItem 报错了）
        if (pools.TryGetValue(prefab, out var pool))
        {
            GameObject obj = pool.Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }

        return null;
    }
    public T Get<T>(GameObject prefab, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        GameObject obj = Get(prefab, position, rotation);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        return null;
    }
    // 4. 废弃原有的 Return 方法，强制规范使用 PoolItem
    // 删除 public void Return(GameObject obj)
}