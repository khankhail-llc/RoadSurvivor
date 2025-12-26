using UnityEngine;
using TMPro;

public class CarStateUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI SpeedText;
    [SerializeField] TextMeshProUGUI CurrentGearText;

    [Header("Meter Limits")]
    [SerializeField] float minMeterSpeed = 1f;
    [SerializeField] float maxMeterSpeed = 240f;

    [Header("Meter Step System")]
    [SerializeField] float boostStep = 2f;      // 7 → 9 → 11 → 13
    [SerializeField] float releaseStep = 4f;    // 180 → 176 → 172
    [SerializeField] float stepInterval = 0.05f;

    float uiSpeed;
    float stepTimer;

    PlayerController car;

    void Awake()
    {
        car = FindFirstObjectByType<PlayerController>();
    }

    void Start()
    {
        // 🔑 START SPEED (1–7 range)
        uiSpeed = Random.Range(3f, 7f);
        UpdateSpeedText();
        UpdateGear();
    }

    void Update()
    {
        if (car == null) return;

        stepTimer += Time.deltaTime;
        if (stepTimer < stepInterval) return;

        stepTimer = 0f;

        if (car.IsBoosting)

        {
            // ACCELERATION
            uiSpeed += boostStep;
        }
        else
        {
            // DECELERATION
            uiSpeed -= releaseStep;
        }

        uiSpeed = Mathf.Clamp(uiSpeed, minMeterSpeed, maxMeterSpeed);

        UpdateSpeedText();
        UpdateGear();
    }

    void UpdateSpeedText()
    {
        SpeedText.text = Mathf.RoundToInt(uiSpeed).ToString("000");
    }

    void UpdateGear()
    {
        if (uiSpeed < 60f)
            SetGear("S", Color.green);
        else if (uiSpeed < 130f)
            SetGear("D", Color.yellow);
        else
            SetGear("L", Color.red);
    }

    void SetGear(string gear, Color color)
    {
        CurrentGearText.text = gear;
        CurrentGearText.color = color;
    }
}
