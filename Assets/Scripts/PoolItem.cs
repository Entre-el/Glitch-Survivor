using UnityEngine;
using UnityEngine.Pool;

public class PoolItem : MonoBehaviour
{
    private IObjectPool<GameObject> myPool;
    public int defaultCapacity = 50;
    public int maxSize = 200;

    // 依赖注入
    public void SetPool(IObjectPool<GameObject> pool)
    {
        myPool = pool;
    }

    public void ReturnToPool()
    {
        if (myPool != null)
        {
            myPool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
