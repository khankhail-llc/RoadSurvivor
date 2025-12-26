using UnityEngine;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform player;
    public float spawnDistanceInterval = 12f;
    public float spawnAheadDistance = 18f;
    public float[] lanesX = { -2.5f, 0f, 2.5f };

    private Queue<GameObject> coinPool = new Queue<GameObject>();
    private float lastSpawnY = -999f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < 10; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false);
            coinPool.Enqueue(coin);
        }
    }

    void Update()
    {
        float currentY = player.position.y;
        if (currentY - lastSpawnY >= spawnDistanceInterval)
        {
            SpawnCoin();
            lastSpawnY = currentY;
        }
    }

    void SpawnCoin()
    {
        GameObject coin = null;

        foreach (GameObject c in coinPool)
        {
            if (!c.activeInHierarchy) { coin = c; break; }
        }

        if (coin == null) return;

        int lane = Random.Range(0, lanesX.Length);
        float spawnY = player.position.y + spawnAheadDistance;

        coin.transform.position = new Vector3(lanesX[lane], spawnY, 0);
        coin.SetActive(true);
    }
}