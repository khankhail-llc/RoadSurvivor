using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mainCamera;

    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs;
    public float spawnInterval = 1.4f;
    public float minGapInSameLane = 8f;

    [Header("Lane Settings (3 Lane Road)")]
    public float[] laneXPositions = new float[3]; // Inspector mai 3 lanes ki X values

    [Header("Pooling Settings")]
    public int poolSize = 25;

    private float timer = 0f;
    private float[] lastSpawnYPerLane;
    private List<GameObject> pool = new List<GameObject>();

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (mainCamera == null)
            mainCamera = Camera.main;

        // Safety check
        if (laneXPositions.Length != 3)
        {
            Debug.LogError("LaneXPositions must contain exactly 3 values!");
            return;
        }

        // Initialize last spawn Y per lane
        lastSpawnYPerLane = new float[laneXPositions.Length];
        for (int i = 0; i < lastSpawnYPerLane.Length; i++)
            lastSpawnYPerLane[i] = -9999f;

        // Initialize pool
        for (int i = 0; i < poolSize; i++)
        {
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
        int lane = Random.Range(0, laneXPositions.Length);
        float spawnY = mainCamera.transform.position.y + mainCamera.orthographicSize + 1f;

        if (spawnY - lastSpawnYPerLane[lane] < minGapInSameLane)
            return;

        GameObject obs = GetFromPool();
        if (obs == null) return;

        obs.transform.position = new Vector3(laneXPositions[lane], spawnY, 0f);
        obs.SetActive(true);
        lastSpawnYPerLane[lane] = spawnY;
    }

    GameObject GetFromPool()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

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
