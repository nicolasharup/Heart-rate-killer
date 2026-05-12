using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    [Header("Terrain")]
    public Terrain terrain;

    [Header("Prefabs")]
    public GameObject[] treePrefabs;
    public GameObject[] stumpPrefabs;

    [Header("Tree Clusters")]
    public int clusterCount = 45;
    public int treesPerCluster = 25;
    public float clusterRadius = 12f;

    [Header("Stumps")]
    public int stumpCount = 40;

    [Header("Random Scale")]
    public float minScale = 0.8f;
    public float maxScale = 1.4f;

    [Header("Tree Wall")]
    public bool generateTreeWall = true;
    public int wallRows = 5;
    public float wallSpacing = 2.5f;
    public float wallJitter = 1f;
    public float wallScaleMultiplier = 1.5f;

    [Header("Campfire Clearings")]
    public string noTreeZoneTag = "NoTreeZone";

    private Collider[] noTreeZones;

    void Start()
    {
        noTreeZones = FindObjectsOfType<Collider>();

        Generate();
    }

    void Generate()
    {
        if (terrain == null)
        {
            Debug.LogError("ForestGenerator: No terrain assigned.");
            return;
        }

        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        SpawnClusters(treePrefabs, clusterCount, treesPerCluster, clusterRadius, data, terrainPos);
        SpawnObjects(stumpPrefabs, stumpCount, data, terrainPos);

        if (generateTreeWall)
            SpawnTreeWall(data, terrainPos);
    }

    void SpawnClusters(
        GameObject[] prefabs,
        int numberOfClusters,
        int objectsPerCluster,
        float radius,
        TerrainData data,
        Vector3 terrainPos)
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        for (int c = 0; c < numberOfClusters; c++)
        {
            float centerX = Random.Range(0, data.size.x);
            float centerZ = Random.Range(0, data.size.z);

            for (int i = 0; i < objectsPerCluster; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * radius;

                float x = centerX + randomCircle.x;
                float z = centerZ + randomCircle.y;

                if (x < 0 || z < 0 || x > data.size.x || z > data.size.z)
                    continue;

                if (InsideNoTreeZone(x, z, terrainPos))
                    continue;

                SpawnPrefab(prefabs, x, z, data, terrainPos, 1f);
            }
        }
    }

    void SpawnObjects(
        GameObject[] prefabs,
        int count,
        TerrainData data,
        Vector3 terrainPos)
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(0, data.size.x);
            float z = Random.Range(0, data.size.z);

            if (InsideNoTreeZone(x, z, terrainPos))
                continue;

            SpawnPrefab(prefabs, x, z, data, terrainPos, 1f);
        }
    }

    void SpawnTreeWall(TerrainData data, Vector3 terrainPos)
    {
        if (treePrefabs == null || treePrefabs.Length == 0)
            return;

        float width = data.size.x;
        float length = data.size.z;

        for (int row = 0; row < wallRows; row++)
        {
            float offset = row * wallSpacing;

            for (float x = 0; x <= width; x += wallSpacing)
            {
                SpawnWallTree(x, offset, data, terrainPos);
                SpawnWallTree(x, length - offset, data, terrainPos);
            }

            for (float z = 0; z <= length; z += wallSpacing)
            {
                SpawnWallTree(offset, z, data, terrainPos);
                SpawnWallTree(width - offset, z, data, terrainPos);
            }
        }
    }

    void SpawnWallTree(float x, float z, TerrainData data, Vector3 terrainPos)
    {
        x += Random.Range(-wallJitter, wallJitter);
        z += Random.Range(-wallJitter, wallJitter);

        x = Mathf.Clamp(x, 0f, data.size.x);
        z = Mathf.Clamp(z, 0f, data.size.z);

        if (InsideNoTreeZone(x, z, terrainPos))
            return;

        SpawnPrefab(treePrefabs, x, z, data, terrainPos, wallScaleMultiplier);
    }

    bool InsideNoTreeZone(float x, float z, Vector3 terrainPos)
    {
        Vector3 worldPos = terrainPos + new Vector3(x, 0f, z);

        foreach (Collider zone in noTreeZones)
        {
            if (zone == null)
                continue;

            if (!zone.CompareTag(noTreeZoneTag))
                continue;

            if (zone.bounds.Contains(worldPos))
                return true;
        }

        return false;
    }

    void SpawnPrefab(
        GameObject[] prefabs,
        float x,
        float z,
        TerrainData data,
        Vector3 terrainPos,
        float scaleMultiplier)
    {
        float y = data.GetInterpolatedHeight(x / data.size.x, z / data.size.z);

        Vector3 position = terrainPos + new Vector3(x, y, z);

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

        GameObject obj = Instantiate(
            prefab,
            position,
            Quaternion.Euler(0f, Random.Range(0f, 360f), 0f),
            transform
        );

        float scale = Random.Range(minScale, maxScale) * scaleMultiplier;

        obj.transform.localScale = Vector3.one * scale;
    }
}