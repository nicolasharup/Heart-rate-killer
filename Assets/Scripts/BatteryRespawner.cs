using UnityEngine;

public class BatteryRespawner : MonoBehaviour
{
    [Header("Battery")]
    public GameObject batteryPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Respawn")]
    public float respawnTime = 45f;

    private GameObject currentBattery;
    private float timer;

    void Start()
    {
        SpawnBattery();
    }

    void Update()
    {
        if (currentBattery != null)
            return;

        timer += Time.deltaTime;

        if (timer >= respawnTime)
        {
            SpawnBattery();
            timer = 0f;
        }
    }

    void SpawnBattery()
    {
        currentBattery = Instantiate(
            batteryPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }
}