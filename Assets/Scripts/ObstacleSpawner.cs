using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mainCamera;

    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs; // all obstacle prefabs
    public float spawnInterval = 1.4f;
    public float minGapInSameLane = 8f;

    [Header("Pooling Settings")]
    public int poolSize = 25;

    private float timer = 0f;
    private float[] laneX;
    private float[] lastSpawnYPerLane;
    private List<GameObject> pool = new List<GameObject>();

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (mainCamera == null)
            mainCamera = Camera.main;

        // Screen width based lanes
        float cameraWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float margin = 0.5f; // obstacle spawn margin from edges
        laneX = new float[] { -cameraWidth + margin, 0f, cameraWidth - margin };

        // Initialize last spawn Y per lane
        lastSpawnYPerLane = new float[laneX.Length];
        for (int i = 0; i < lastSpawnYPerLane.Length; i++)
            lastSpawnYPerLane[i] = -9999f;

        // Initialize pool (any prefab)
        for (int i = 0; i < poolSize; i++)
        {
            // Pick a random prefab
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);

            if (obj.GetComponent<Obstacle>() == null)
                obj.AddComponent<Obstacle>();

            pool.Add(obj);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnOneObstacle();
            timer = 0f;
        }
    }

    void SpawnOneObstacle()
    {
        int lane = Random.Range(0, laneX.Length);
        float spawnY = mainCamera.transform.position.y + mainCamera.orthographicSize + 1f;

        if (spawnY - lastSpawnYPerLane[lane] < minGapInSameLane)
            return;

        GameObject obs = GetFromPool();
        if (obs == null) return;

        obs.transform.position = new Vector3(laneX[lane], spawnY, 0f);
        obs.SetActive(true);
        lastSpawnYPerLane[lane] = spawnY;
    }

    GameObject GetFromPool()
    {
        // Pick first inactive object from pool
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                // Replace with new random prefab
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

                // Destroy old object and replace with new prefab
                Vector3 pos = obj.transform.position;
                pool.Remove(obj);
                Destroy(obj);

                GameObject newObj = Instantiate(prefab, pos, Quaternion.identity);
                if (newObj.GetComponent<Obstacle>() == null)
                    newObj.AddComponent<Obstacle>();

                pool.Add(newObj);
                return newObj;
            }
        }

        // Pool exhausted → create new random prefab
        GameObject newObj2 = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]);
        if (newObj2.GetComponent<Obstacle>() == null)
            newObj2.AddComponent<Obstacle>();

        pool.Add(newObj2);
        return newObj2;
    }

    void LateUpdate()
    {
        float bottomY = mainCamera.transform.position.y - mainCamera.orthographicSize - 1f;

        foreach (GameObject obj in pool)
        {
            if (obj.activeInHierarchy && obj.transform.position.y < bottomY)
                obj.SetActive(false);
        }
    }
}
