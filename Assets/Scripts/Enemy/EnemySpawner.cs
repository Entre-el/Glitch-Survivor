using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{ 
    [Header("Audio")]
    public AudioClip finalWaveBGM;
    public float cutInDuration = 2f;
    public static EnemySpawner Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    currentGroupCount = 0;
    currentWaveCount = 0;
    }
    [System.Serializable]
    public class EnemyGroup
    {
        [System.Serializable]
        public class Enemies
        {
            public GameObject enemyPrefab;
            public int enemyCount;
        }
        public List<Enemies> enemysList;
        public bool isGathered;
        public int difficultyLevel;
    }
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public EnemyGroup[] enemyGroups;
    }
    public float waveSpawnInterval;
    [SerializeField]
    private float waveSpawnTimer;
    public float groupSpawnInterval;
    [SerializeField]
    private float groupSpawnTimer;
    [SerializeField]
    private int currentWaveCount;
    [SerializeField]
    private int currentGroupCount;

    [Header("Spawn Attributes")]
    private float enemiesAlive;
    public int maxEnemiesAllowed;
    public int roundEdgeLength;
    public List<Wave> waves;
    private bool isDone = false;
    Transform player;
    PlayerStats playerStats;
    void Start()
    {
        isDone = false;
        enemiesAlive = 0;
        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;
        // 游戏开始，直接启动刷怪流水线！
        StartCoroutine(SpawnProcess());
        playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if(isDone)
        {
            if(enemiesAlive <= 0)
            {
                playerStats.Win();
            }
        }
    }
    private IEnumerator SpawnProcess()
    {
        // 第一层：遍历所有的波次
        for (int w = 0; w < waves.Count; w++)
        {
            Wave currentWave = waves[w];
            Debug.Log($"开始生成波次：{currentWave.waveName}");

            // 第二层：遍历这一波里的所有怪物组
            for (int g = 0; g < currentWave.enemyGroups.Length; g++)
            {
                EnemyGroup currentGroup = currentWave.enemyGroups[g];
                if(enemiesAlive >= maxEnemiesAllowed)
                {
                    Debug.Log("当前场上怪物过多，等待怪物数量下降后继续生成...");
                    // 等待直到场上怪物数量降到允许范围内
                    yield return new WaitUntil(() => enemiesAlive < maxEnemiesAllowed);
                }
                // 把当前组直接传给生成函数
                SpawnEnemyGroup(currentGroup);

                // 暂停组与组之间的间隔时间
                yield return new WaitForSeconds(groupSpawnInterval); 
                currentGroupCount++;
            }
            currentGroupCount = 0;
            // 波与波之间的休息时间
            yield return new WaitForSeconds(waveSpawnInterval);
            currentWaveCount++;
            if(currentWaveCount == waves.Count - 1)
            {
                EventCenter.Broadcast(EventDefine.OnBossDied);
            }
        }
        Debug.Log($"所有波次的怪物生成完毕");
        isDone = true;
    }
    private void SpawnEnemyGroup(EnemyGroup group)
    {
        if (group.isGathered)
        {
            Vector2 spawnPosition = GetRoundEnemyPosition(roundEdgeLength);
            foreach (var enemies in group.enemysList)
            {
                for (int i = 0; i < enemies.enemyCount; i++)
                {
                    GameObject enemy = ObjectPoolManager.Instance.Get(enemies.enemyPrefab, spawnPosition);
                    enemy.transform.position = spawnPosition+new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                    enemy.SetActive(true);
                    enemiesAlive++;
                }
            }
        }
        else
        {
            foreach (var singleGroup in group.enemysList)
            {
                for (int i = 0; i < singleGroup.enemyCount; i++)
                {
                    Vector2 randomSpawnPosition = GetRoundEnemyPosition(roundEdgeLength);
                    GameObject enemy = ObjectPoolManager.Instance.Get(singleGroup.enemyPrefab, randomSpawnPosition+new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f)), Quaternion.identity);
                    enemy.transform.position = randomSpawnPosition+new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
                    enemy.SetActive(true);
                    enemiesAlive++;
                }
            }
        }
    }
    public Vector2 GetRoundEnemyPosition(int edgeLength)
    {
        int edge = Random.Range(0, 4);
        float randomPos = Random.Range(-1f,1f);
        Vector2 spawnPosition;
        switch (edge)
        {
        case 0:
            spawnPosition = new Vector2(player.position.x + edgeLength*randomPos, player.position.y + edgeLength);
            break;
        case 1:
            spawnPosition = new Vector2(player.position.x + edgeLength*randomPos, player.position.y - edgeLength);
            break;
        case 2:
            spawnPosition = new Vector2(player.position.x + edgeLength, player.position.y + edgeLength*randomPos);
            break;
        case 3:
            spawnPosition = new Vector2(player.position.x - edgeLength, player.position.y + edgeLength*randomPos);
            break;
        default:
            throw new System.Exception($"Unexpected edge value: {edge}");
        }
        return spawnPosition;
    }
    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
}
 