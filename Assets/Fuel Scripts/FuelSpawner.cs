using UnityEngine;
using System.Collections.Generic;

public class FuelSpawner : MonoBehaviour
{
    public GameObject fuelCanPrefab;
    public Transform player;
    public float spawnDistanceInterval = 12f;
    public float spawnAheadDistance = 18f;
    public float[] lanesX = { -2.5f, 0f, 2.5f };

    private Queue<GameObject> fuelPool = new Queue<GameObject>();
    private float lastSpawnY = -999f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < 10; i++)
        {
            GameObject fuel = Instantiate(fuelCanPrefab);
            fuel.SetActive(false);
            fuelPool.Enqueue(fuel);
        }
    }

    void Update()
    {
        float currentY = player.position.y;
        if (currentY - lastSpawnY >= spawnDistanceInterval)
        {
            SpawnFuelCan();
            lastSpawnY = currentY;
        }
    }

    void SpawnFuelCan()
    {
        GameObject fuel = null;

        foreach (GameObject f in fuelPool)
        {
            if (!f.activeInHierarchy) { fuel = f; break; }
        }

        if (fuel == null) return;

        int lane = Random.Range(0, lanesX.Length);
        float spawnY = player.position.y + spawnAheadDistance;

        fuel.transform.position = new Vector3(lanesX[lane], spawnY, 0);
        fuel.SetActive(true);
    }
}
