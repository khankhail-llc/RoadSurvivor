using UnityEngine;
using System.Collections.Generic;

public class InfiniteRoadSpawner : MonoBehaviour
{
    [Header("Road Settings")]
    public GameObject roadPrefab;
    public int roadsToKeep = 8;
    public float spawnAheadDistance = 30f;
    public float recycleBehindDistance = 40f;
    public Transform playerTransform;

    [Header("Speed Up Settings")]
    public float accelerationRate = 0.01f; // per second
    public float minSpawnAheadDistance = 15f; // minimum distance for high speed

    private List<GameObject> activeRoads = new List<GameObject>();
    private Queue<GameObject> roadPool = new Queue<GameObject>();
    private float roadLength;
    private float lastSpawnY;

    void Start()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (roadPrefab == null || playerTransform == null)
        {
            Debug.LogError("Assign RoadPrefab and Player!");
            enabled = false;
            return;
        }

        roadLength = roadPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size.y;

        // Initialize pool
        for (int i = 0; i < roadsToKeep + 5; i++)
        {
            GameObject road = Instantiate(roadPrefab);
            road.SetActive(false);
            roadPool.Enqueue(road);
        }

        float startY = Mathf.Floor(playerTransform.position.y / roadLength) * roadLength;
        lastSpawnY = startY - roadLength;

        for (int i = 0; i < roadsToKeep; i++)
            SpawnRoadAhead();
    }

    void Update()
    {
        // Gradually increase spawn speed
        spawnAheadDistance -= accelerationRate * Time.deltaTime;
        if (spawnAheadDistance < minSpawnAheadDistance)
            spawnAheadDistance = minSpawnAheadDistance;

        float playerY = playerTransform.position.y;

        // Spawn roads ahead
        while (lastSpawnY < playerY + spawnAheadDistance)
            SpawnRoadAhead();

        // Recycle old roads
        while (activeRoads.Count > 0 &&
               activeRoads[0].transform.position.y + roadLength < playerY - recycleBehindDistance)
            RecycleRoad(activeRoads[0]);
    }

    void SpawnRoadAhead()
    {
        if (roadPool.Count == 0) return;

        GameObject road = roadPool.Dequeue();
        lastSpawnY += roadLength;
        road.transform.position = new Vector3(0, lastSpawnY, 0);
        road.SetActive(true);
        activeRoads.Add(road);
    }

    void RecycleRoad(GameObject road)
    {
        activeRoads.RemoveAt(0);
        road.SetActive(false);
        roadPool.Enqueue(road);
    }
}

