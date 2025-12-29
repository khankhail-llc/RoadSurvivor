using UnityEngine;

/// <summary>
/// DEVELOPMENT ONLY - Testing ke liye cars reset karne ka button
/// Production mein is script ko REMOVE kar dena!
/// </summary>
public class DevResetButton : MonoBehaviour
{
    [Header("Development Testing")] 
    [Tooltip("Total cars in game")]
    [SerializeField] private int totalCars = 10;

    /// <summary>
    /// Sab cars ko lock kar do (except first car)
    /// Button ke OnClick event se call karo
    /// </summary>
    public void ResetAllCars()
    {
        Debug.Log("[DEV] Resetting all cars except first car...");
        
        CarUnlockManager.Instance.ResetAllCars(totalCars);
        
        Debug.Log("[DEV] All cars reset! Only car 0 is unlocked.");
        
        // Optional: Agar CarSelection scene mein hai, UI update karo
        CarSelection carSelection = FindObjectOfType<CarSelection>();
        if (carSelection != null)
        {
            // Private method access nahi hai, toh scene reload kar sakte ho
            Debug.Log("[DEV] Restart scene to see changes in UI");
        }
    }

    /// <summary>
    /// Sab cars unlock kar do - testing ke liye
    /// </summary>
    public void UnlockAllCars()
    {
        Debug.Log("[DEV] Unlocking all cars...");
        
        CarUnlockManager.Instance.UnlockAllCars(totalCars);
        
        Debug.Log("[DEV] All cars unlocked!");
    }

    /// <summary>
    /// PlayerPrefs completely clear kar do
    /// WARNING: Ye sab saved data delete kar dega!
    /// </summary>
    public void ClearAllPlayerPrefs()
    {
        Debug.LogWarning("[DEV] Clearing ALL PlayerPrefs data...");
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("[DEV] All PlayerPrefs cleared!");
    }

    // Visual reminder ke ye development script hai
    private void OnValidate()
    {
        gameObject.name = "[DEV] Reset Button - REMOVE BEFORE RELEASE!";
    }
}
