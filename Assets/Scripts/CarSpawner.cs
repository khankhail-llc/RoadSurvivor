// using UnityEngine;

// public class CarSpawner : MonoBehaviour
// {
//     [SerializeField] GameObject[] carsPrefabs;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         SpawnCar();
//     }

//     // Update is called once per frame
//    void SpawnCar()
//    {
//        int CurrentCarIndex = PlayerPrefs.GetInt("SelectedCar", 0);
//        Instantiate(carsPrefabs[CurrentCarIndex], transform.position, Quaternion.identity);
//    }
// }
