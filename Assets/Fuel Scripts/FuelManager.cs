using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FuelManager : MonoBehaviour
{
    public static FuelManager Instance;

    [Header("UI Elements")]
    public Slider fuelBar;
    public TextMeshProUGUI fuelText;

    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float fuelDepletionRate = 5f;

    private float currentFuel;
    private bool isGameOver = false;

    // ⭐ NEW → Check if out of fuel caused GameOver
    public bool isFuelFinished = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += HideFuelUI;
            GameManager.Instance.OnGameRestart += ShowFuelUI;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= HideFuelUI;
            GameManager.Instance.OnGameRestart -= ShowFuelUI;
        }
    }

    private void Start()
    {
        currentFuel = maxFuel;
        ShowFuelUI();
    }

    private void Update()
    {
        if (isGameOver) return;

        currentFuel -= fuelDepletionRate * Time.deltaTime;

        if (currentFuel <= 0)
        {
            currentFuel = 0;

            // ⭐ Fuel finished → mark reason
            isFuelFinished = true;

            GameManager.Instance?.GameOver();
        }

        UpdateFuelUI();
    }

    public void CollectFuel()
    {
        if (isGameOver) return;

        currentFuel = maxFuel;
        UpdateFuelUI();
    }

    private void UpdateFuelUI()
    {
        if (fuelBar != null)
            fuelBar.value = currentFuel / maxFuel;

        if (fuelText != null)
            fuelText.text = "" + Mathf.RoundToInt(currentFuel) + "%";
    }

    private void HideFuelUI()
    {
        isGameOver = true;

        if (fuelBar != null) fuelBar.gameObject.SetActive(false);
        if (fuelText != null) fuelText.gameObject.SetActive(false);
    }

    private void ShowFuelUI()
    {
        isGameOver = false;

        if (fuelBar != null)
        {
            fuelBar.gameObject.SetActive(true);
            fuelBar.value = currentFuel / maxFuel;
        }

        if (fuelText != null)
        {
            fuelText.gameObject.SetActive(true);
            fuelText.text = "Fuel: " + Mathf.RoundToInt(currentFuel) + "%";
        }
    }

    // 🔥 Hide when pause or settings
    public void HideFuelTemporarily()
    {
        if (fuelBar != null) fuelBar.gameObject.SetActive(false);
        if (fuelText != null) fuelText.gameObject.SetActive(false);
    }

    public void ShowFuelAfterMenu()
    {
        if (fuelBar != null) fuelBar.gameObject.SetActive(true);
        if (fuelText != null) fuelText.gameObject.SetActive(true);
    }

    // ⭐ RESET EVERYTHING on Restart
    public void ResetFuel()
    {
        isGameOver = false;
        currentFuel = maxFuel;

        // ⭐ Reset reason
        isFuelFinished = false;

        UpdateFuelUI();
        ShowFuelUI();
    }
}

