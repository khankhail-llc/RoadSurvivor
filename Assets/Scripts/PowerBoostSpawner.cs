// using UnityEngine;
// using System.Collections.Generic;

// public class PowerBoostSpawner : MonoBehaviour
// {
//     [Header("Assign in Inspector")]
//     public GameObject boostPrefab;          // Boost prefab
//     public Transform player;                // Player transform
//     public float spawnDistanceInterval = 50f; // Distance baad spawn
//     public float spawnAheadDistance = 20f;    // Player se kitna aage spawn
//     public float[] lanesX = { -2.5f, 0f, 2.5f }; // Lane positions

//     [Header("Object Pool Settings")]
//     public int poolSize = 5;                // Kitne boosts pool me
//     private Queue<GameObject> boostPool = new Queue<GameObject>();

//     private float lastSpawnY = -999f;

//     void Start()
//     {
//         if (player == null)
//             player = GameObject.FindGameObjectWithTag("Player").transform;

//         // Object pool create
//         for (int i = 0; i < poolSize; i++)
//         {
//             GameObject boost = Instantiate(boostPrefab);
//             boost.SetActive(false);
//             boostPool.Enqueue(boost);
//         }
//     }

//     void Update()
//     {
//         float currentY = player.position.y;

//         if (currentY - lastSpawnY >= spawnDistanceInterval)
//         {
//             SpawnBoost();
//             lastSpawnY = currentY;
//         }
//     }

//     void SpawnBoost()
//     {
//         GameObject boost = null;

//         foreach (GameObject b in boostPool)
//         {
//             if (!b.activeInHierarchy)
//             {
//                 boost = b;
//                 break;
//             }
//         }

//         if (boost == null) return;

//         int lane = Random.Range(0, lanesX.Length);
//         float spawnY = player.position.y + spawnAheadDistance;

//         boost.transform.position = new Vector3(lanesX[lane], spawnY, 0);
//         boost.SetActive(true);
//     }
// }











using UnityEngine;
using System.Collections.Generic;

public class PowerBoostSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject boostPrefab;          // Boost prefab
    public Transform player;                // Player transform
    public float spawnDistanceInterval = 50f; // Distance baad spawn
    public float spawnAheadDistance = 20f;    // Player se kitna aage spawn
    public float[] lanesX = { -2.5f, 0f, 2.5f }; // Lane positions

    [Header("Object Pool Settings")]
    public int poolSize = 5;                // Kitne boosts pool me
    private Queue<GameObject> boostPool = new Queue<GameObject>();

    private float lastSpawnY = -999f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Object pool create
        for (int i = 0; i < poolSize; i++)
        {
            GameObject boost = Instantiate(boostPrefab);
            boost.SetActive(false);
            boostPool.Enqueue(boost);
        }
    }

    void Update()
    {
        float currentY = player.position.y;

        if (currentY - lastSpawnY >= spawnDistanceInterval)
        {
            SpawnBoost();
            lastSpawnY = currentY;
        }
    }

    void SpawnBoost()
    {
        GameObject boost = null;

        foreach (GameObject b in boostPool)
        {
            if (!b.activeInHierarchy)
            {
                boost = b;
                break;
            }
        }

        if (boost == null) return;

        int lane = Random.Range(0, lanesX.Length);
        float spawnY = player.position.y + spawnAheadDistance;

        boost.transform.position = new Vector3(lanesX[lane], spawnY, 0);
        boost.SetActive(true);
    }
}