using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("核心引用")]
    [Tooltip("要生成的怪物预制体")]
    public GameObject enemyPrefab;

    [Tooltip("玩家的 Transform，怪物将围绕玩家生成")]
    public Transform playerTransform;

    [Header("生成规则")]
    [Tooltip("同屏最大怪物数量上限")]
    public int maxEnemiesOnScreen = 1000;

    [Tooltip("每次触发生成的怪物数量（分摊 CPU 压力）")]
    public int spawnCountPerTick = 5;

    [Tooltip("生成间隔时间（秒）")]
    public float spawnInterval = 0.1f;

    [Header("生成范围 (视野外环形)")]
    [Tooltip("最小生成半径（保证不刷在屏幕内贴脸）")]
    public float minSpawnRadius = 15f;

    [Tooltip("最大生成半径")]
    public float maxSpawnRadius = 20f;

    private float timer;
    private int currentActiveEnemies = 0;

    private void OnEnable()
    {
        // 🌟 监听怪物死亡事件，动态扣减同屏数量
        // （结合你之前 EnemyCore 里的 EventCenter.Broadcast(EventDefine.OnEnemyDied, this)）
        EventCenter.AddListener<EnemyCore>(EventDefine.OnEnemyDied, OnEnemyDied);
    }

    private void OnDisable()
    {
        EventCenter.RemoveListener<EnemyCore>(EventDefine.OnEnemyDied, OnEnemyDied);
    }

    private void Update()
    {
        if (playerTransform == null || enemyPrefab == null)
            return;

        timer += Time.deltaTime;

        // 如果时间到了，并且当前同屏怪物没达到上限，就开始刷怪
        if (timer >= spawnInterval && currentActiveEnemies < maxEnemiesOnScreen)
        {
            timer = 0f;
            SpawnBatch();
        }
    }

    private void SpawnBatch()
    {
        // 计算这次到底能刷多少只（防止最后一次刷怪超出上限）
        int spawnCount = Mathf.Min(spawnCountPerTick, maxEnemiesOnScreen - currentActiveEnemies);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = GetRandomOffScreenPosition();

            // 🌟 核心：绝对不能用 Instantiate，必须从你的对象池中获取！
            GameObject enemyObj = ObjectPoolManager.Instance.Get(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );

            if (enemyObj != null)
            {
                currentActiveEnemies++;
            }
        }
    }

    // 🌟 获取屏幕外的随机环形坐标
    private Vector3 GetRandomOffScreenPosition()
    {
        // 随机生成一个 2D 方向向量
        Vector2 randomDir = Random.insideUnitCircle.normalized;

        // 随机生成一个距离（在最小和最大半径之间）
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // 以玩家为中心，加上方向和距离，得出最终坐标
        Vector3 spawnPos =
            playerTransform.position + new Vector3(randomDir.x, randomDir.y, 0f) * randomDistance;

        return spawnPos;
    }

    // 怪物死亡时的回调
    private void OnEnemyDied(EnemyCore deadEnemy)
    {
        currentActiveEnemies--;
        // 防止出现负数异常
        currentActiveEnemies = Mathf.Max(0, currentActiveEnemies);
    }

    // （可选）在 Scene 视图中画出刷怪范围，方便你调整数值
    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minSpawnRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, maxSpawnRadius);
        }
    }
}
