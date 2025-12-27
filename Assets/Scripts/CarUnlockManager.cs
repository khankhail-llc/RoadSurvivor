using UnityEngine;

/// <summary>
/// Car unlock states ko manage karta hai aur PlayerPrefs use karke save karta hai
/// Pehli car (index 0) default unlocked hoti hai
/// </summary>
public class CarUnlockManager : MonoBehaviour
{
    private static CarUnlockManager instance;
    
    public static CarUnlockManager Instance 
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CarUnlockManager");
                instance = go.AddComponent<CarUnlockManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Check karta hai ke car unlocked hai ya nahi
    /// </summary>
    public bool IsCarUnlocked(int carIndex)
    {
        // Pehli car (index 0) hamesha unlocked
        if (carIndex == 0)
            return true;

        // Baaki cars ke liye PlayerPrefs se check karo
        string key = "CarUnlocked_" + carIndex;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    /// <summary>
    /// Car ko unlock karta hai aur save karta hai
    /// </summary>
    public void UnlockCar(int carIndex)
    {
        string key = "CarUnlocked_" + carIndex;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
        
        Debug.Log("Car " + carIndex + " unlocked!");
    }

    /// <summary>
    /// Development ke liye: sab cars unlock kar do
    /// </summary>
    public void UnlockAllCars(int totalCars)
    {
        for (int i = 0; i < totalCars; i++)
        {
            UnlockCar(i);
        }
        Debug.Log("All cars unlocked!");
    }

    /// <summary>
    /// Development ke liye: sab cars lock kar do (except first car)
    /// </summary>
    public void ResetAllCars(int totalCars)
    {
        for (int i = 1; i < totalCars; i++)
        {
            string key = "CarUnlocked_" + i;
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
        Debug.Log("All cars reset (except first car)!");
    }
}
