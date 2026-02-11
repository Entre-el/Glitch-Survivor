using System.Collections.Generic; // 必须引用这个，才能用 Dictionary
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("设置")]
    public GameObject[] terrainChunks; // 地图块预制体数组
    public Transform player;           // 玩家位置
    public float chunkSize = 20f;      // 必须和你地图Prefab的实际大小一致！
    public int chunkRadius = 1;        // 视野半径：1表示九宫格(3x3)，2表示5x5
    public int disableRadius = 3;
    [Header("调试信息")]
    public Vector2 currentChunkCoord;  // 当前玩家在哪一个格子里

    // 核心数据结构：字典
    // Key (Vector2): 地图的“网格坐标”，比如 (1, 1)
    // Value (GameObject): 实际生成的地图物体
    // 作用：用来快速查找“这里有没有生成过地图”
    private Dictionary<Vector2, GameObject> loadedChunks = new Dictionary<Vector2, GameObject>();

    void Start()
    {
        // 初始生成一次
        UpdateChunks();
    }

    void Update()
    {
        // 每帧检查玩家位置是否变化到了新的格子里
        Vector2 playerCoord = GetChunkCoordFromVector3(player.position);

        // 只有当玩家跨过格子的边界时，才更新地图 (优化性能)
        if (playerCoord != currentChunkCoord)
        {
            currentChunkCoord = playerCoord;
            UpdateChunks();
        }
    }

    void UpdateChunks()
    {
        // 遍历周围的格子
        // x 从 -1 到 +1，y 从 -1 到 +1 (如果 radius 是 1)
        for (int xOffset = -chunkRadius; xOffset <= chunkRadius; xOffset++)
        {
            for (int yOffset = -chunkRadius; yOffset <= chunkRadius; yOffset++)
            {
                // 算出目标格子的坐标。比如玩家在(5,5)，偏移(-1,0)，目标就是(4,5)
                Vector2 targetCoord = new Vector2(currentChunkCoord.x + xOffset, currentChunkCoord.y + yOffset);

                // 检查：这个坐标生成过吗？
                if (!loadedChunks.ContainsKey(targetCoord))
                {
                    SpawnChunk(targetCoord);
                }
                else if(loadedChunks.ContainsKey(targetCoord))
                {
                    loadedChunks[targetCoord].SetActive(true);
                }
            }
        }
        foreach(GameObject chunk in loadedChunks.Values)
        {
            if(chunk.transform.position.x < player.position.x - chunkSize * disableRadius ||
            chunk.transform.position.x > player.position.x + chunkSize * disableRadius ||
            chunk.transform.position.y < player.position.y - chunkSize * disableRadius ||
            chunk.transform.position.y > player.position.y + chunkSize * disableRadius)
            {
                chunk.SetActive(false);
            }
        }  
    }

    void SpawnChunk(Vector2 gridCoord)
    {
        // 1. 随机选一个地图块
        int rand = Random.Range(0, terrainChunks.Length);

        // 2. 把“网格坐标”转回“世界坐标”
        // 比如网格 (2, 3)，大小 20 -> 世界坐标 (40, 60, 0)
        Vector3 spawnPos = new Vector3(gridCoord.x * chunkSize, gridCoord.y * chunkSize, 0);

        // 3. 生成
        GameObject newChunk = Instantiate(terrainChunks[rand], spawnPos, Quaternion.identity);
        
        // 4. 把新生成的地图放在一个父物体下，保持Hierarchy整洁 (可选)
        newChunk.transform.SetParent(this.transform); 

        // 5. 记在小本本(字典)上，防止下次重复生成
        loadedChunks.Add(gridCoord, newChunk);
    }

    // 辅助函数：把世界坐标 (15.5, 22.1) 转换成 网格坐标 (1, 1)
    Vector2 GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x / chunkSize);
        int y = Mathf.RoundToInt(pos.y / chunkSize);
        return new Vector2(x, y);
    }
}