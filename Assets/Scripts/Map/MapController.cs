using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    [System.Serializable]
    public class PropData
    {
        public string propTag;
        public float propWeight;
    }

    [System.Serializable]
    public class TileBases
    {
        public string tag;
        public TileBase tileBase;
        public float spawnChance;
        [Range(0f, 1f)]
        public float propDensity;
        public List<PropData> allowedProps;
    }

    private class ChunkProps
    {
        public List<GameObject> spawnedProps = new List<GameObject>();
        public List<string> propTags = new List<string>();
    }

    [Header("References")]
    public Transform player;
    public Tilemap globalTilemap;

    [Header("Map Settings")]
    public float chunkSize = 20f;    
    public int chunkRadius = 1;
    public int disableRadius = 2;
    public float noiseScale = 0.05f; 
    public List<TileBases> commonBiomes = new();

    [Header("Rare Biome Settings")]
    public float patchNoiseScale = 0.08f;
    [Range(0f, 1f)]
    public float rarePatchThreshold = 0.85f;
    public List<TileBases> rareBiomes = new();

    private float commonTotalChance = 0;
    private float rareTotalChance = 0;
    public int biomeResolution = 2;
    
    private float seedX;
    private float seedY;

    [Header("Debug")]
    public Vector2 currentChunkCoord;

    private Dictionary<Vector2, ChunkProps> loadedChunks = new();

    void Start()
    {
        seedX = Random.Range(-100000f, 100000f);
        seedY = Random.Range(-100000f, 100000f);

        foreach (var tile in commonBiomes) commonTotalChance += tile.spawnChance;
        foreach (var tile in rareBiomes) rareTotalChance += tile.spawnChance;

        Vector2 playerCoord = GetChunkCoordFromVector3(player.position);
        currentChunkCoord = playerCoord;
        UpdateChunks();
    }

    void Update()
    {
        Vector2 playerCoord = GetChunkCoordFromVector3(player.position);
        if (playerCoord != currentChunkCoord)
        {
            currentChunkCoord = playerCoord;
            UpdateChunks();
        }
    }

    void UpdateChunks()
    {
        for (int xOffset = -chunkRadius; xOffset <= chunkRadius; xOffset++)
        {
            for (int yOffset = -chunkRadius; yOffset <= chunkRadius; yOffset++)
            {
                Vector2 targetCoord = new(currentChunkCoord.x + xOffset, currentChunkCoord.y + yOffset);
                if (!loadedChunks.ContainsKey(targetCoord))
                {
                    SpawnChunk(targetCoord);
                }
            }
        }

        List<Vector2> chunksToRemove = new List<Vector2>();

        foreach (var kvp in loadedChunks)
        {
            Vector2 gridCoord = kvp.Key;
            ChunkProps chunkProps = kvp.Value;

            float chunkWorldX = gridCoord.x * chunkSize;
            float chunkWorldY = gridCoord.y * chunkSize;

            if (chunkWorldX < player.position.x - chunkSize * disableRadius ||
                chunkWorldX > player.position.x + chunkSize * disableRadius ||
                chunkWorldY < player.position.y - chunkSize * disableRadius ||
                chunkWorldY > player.position.y + chunkSize * disableRadius)
            {
                EraseChunkTiles(gridCoord);

                for (int i = 0; i < chunkProps.spawnedProps.Count; i++)
                {
                    GameObject propToReturn = chunkProps.spawnedProps[i];
                    string tagToReturn = chunkProps.propTags[i];

                    if (propToReturn != null)
                    {
                        propToReturn.transform.position = Vector3.zero;
                        propToReturn.transform.rotation = Quaternion.identity;
                        ObjectPoolManager.Instance.Release(tagToReturn, propToReturn);
                    }
                }

                chunksToRemove.Add(gridCoord);
            }
        }

        foreach (Vector2 key in chunksToRemove)
        {
            loadedChunks.Remove(key);
        }  
    }

    void SpawnChunk(Vector2 gridCoord)
    {
        int startX = Mathf.FloorToInt(gridCoord.x * chunkSize - chunkSize / 2f);
        int startY = Mathf.FloorToInt(gridCoord.y * chunkSize - chunkSize / 2f);
        int size = Mathf.RoundToInt(chunkSize);

        BoundsInt area = new BoundsInt(startX, startY, 0, size, size, 1);
        TileBase[] tileArray = new TileBase[size * size];
        ChunkProps newChunkProps = new ChunkProps();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int absoluteX = startX + x;
                int absoluteY = startY + y;

                int snappedX = Mathf.FloorToInt((float)absoluteX / biomeResolution) * biomeResolution;
                int snappedY = Mathf.FloorToInt((float)absoluteY / biomeResolution) * biomeResolution;

                float worldX = snappedX * noiseScale + seedX;
                float worldY = snappedY * noiseScale + seedY;
                float baseNoise = Mathf.PerlinNoise(worldX, worldY);

                float patchWorldX = snappedX * patchNoiseScale + seedX + 99999f;
                float patchWorldY = snappedY * patchNoiseScale + seedY + 99999f;
                float patchNoise = Mathf.PerlinNoise(patchWorldX, patchWorldY);

                TileBases selectedBiome;

                if (patchNoise > rarePatchThreshold && rareBiomes.Count > 0)
                {
                    float mappedNoise = baseNoise * rareTotalChance;
                    float currentCumulative = 0f;
                    selectedBiome = rareBiomes[0];
                    
                    foreach (var tile in rareBiomes)
                    {
                        currentCumulative += tile.spawnChance; 
                        if (mappedNoise <= currentCumulative)
                        {
                            selectedBiome = tile;
                            break; 
                        }
                    }
                }
                else
                {
                    float mappedNoise = baseNoise * commonTotalChance;
                    float currentCumulative = 0f;
                    selectedBiome = commonBiomes[0];
                    
                    foreach (var tile in commonBiomes)
                    {
                        currentCumulative += tile.spawnChance; 
                        if (mappedNoise <= currentCumulative)
                        {
                            selectedBiome = tile;
                            break; 
                        }
                    }
                }

                tileArray[x + y * size] = selectedBiome.tileBase;

                if (selectedBiome.allowedProps != null && selectedBiome.allowedProps.Count > 0)
                {
                    if (Random.value <= selectedBiome.propDensity) 
                    {
                        float totalPropWeight = 0;
                        foreach (var p in selectedBiome.allowedProps) totalPropWeight += p.propWeight;
                        
                        float randomPropHit = Random.value * totalPropWeight;
                        float currentPropCumulative = 0f;
                        string finalPropTag = selectedBiome.allowedProps[0].propTag;

                        foreach (var p in selectedBiome.allowedProps)
                        {
                            currentPropCumulative += p.propWeight;
                            if (randomPropHit <= currentPropCumulative)
                            {
                                finalPropTag = p.propTag;
                                break;
                            }
                        }

                        GameObject propObj = ObjectPoolManager.Instance.Get(finalPropTag);
                        if (propObj != null)
                        {
                            Vector3Int cellPos = new Vector3Int(startX + x, startY + y, 0);
                            Vector3 cellCenterWorldPos = globalTilemap.GetCellCenterWorld(cellPos);
                            propObj.transform.position = cellCenterWorldPos;
                        }
                    }
                }
            }
        }

        globalTilemap.SetTilesBlock(area, tileArray);
        loadedChunks.Add(gridCoord, newChunkProps);
    }

    void EraseChunkTiles(Vector2 gridCoord)
    {
        int startX = Mathf.FloorToInt(gridCoord.x * chunkSize - chunkSize / 2f);
        int startY = Mathf.FloorToInt(gridCoord.y * chunkSize - chunkSize / 2f);
        int size = Mathf.RoundToInt(chunkSize);

        BoundsInt area = new BoundsInt(startX, startY, 0, size, size, 1);
        TileBase[] nullArray = new TileBase[size * size];
        globalTilemap.SetTilesBlock(area, nullArray);
    }

    Vector2 GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / chunkSize);
        int y = Mathf.FloorToInt(pos.y / chunkSize);
        return new Vector2(x, y);
    }
}
