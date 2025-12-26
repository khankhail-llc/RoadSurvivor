using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ================= SPEED SETTINGS =================
    [Header("Speed Settings")]
    public float normalSpeed = 2f;
    public float boostSpeed = 10f;
    public float forwardSpeed = 2f;
    public float baseSpeed = 0.5f;
    public float accelerationRate = 1f;
    public float decelerationRate = 0.1f;

    // ================= LANE SETTINGS =================
    [Header("Lane Settings")]
    [Range(0.6f, 0.9f)] public float laneScreenPadding = 0.65f;
    public float laneSmoothTime = 0.07f;
    public int currentLane = 1;

    float[] lanePositions = new float[3];
    Rigidbody2D rb;
    float smoothXVelocity;

    // ================= VISUAL ROTATION =================
    [Header("Visual Rotation (Top-Down)")]
    [SerializeField] private Transform carVisual;
    public float maxTiltAngle = 12f;
    public float rotationSmoothSpeed = 12f;
    float targetRotationZ = 0f;

    // ================= SWIPE =================
    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;
    public float swipeCooldownTime = 0.15f;
    Vector2 startTouchPos;
    bool swipeOnCooldown;

    // ================= SOUND =================
    [Header("Sound Settings")]
    public AudioClip deadSound;
    public AudioClip collectSound;
    AudioSource audioSource;

    [Header("Car Sound Controller")]
    public CarSoundController carSoundController;

    // ================= STATE =================
    [HideInInspector] public bool isDead;
    bool isBoosting;
    public bool IsBoosting => isBoosting;

    float displayedSpeed;

    [Header("CarStateUI")]
    [SerializeField] float maxRPM = 8000f;
    [SerializeField] float maxSpeedKmh = 240f;
    bool lanesCalculated;
    public int DisplayedSpeed => Mathf.RoundToInt(displayedSpeed);
    public float EngineRPM => Mathf.Clamp01(DisplayedSpeed / maxSpeedKmh) * maxRPM;

    // ================= SHIELD =================
    [Header("Shield Settings")]
    public bool isShieldActive = false;
    public float totalShieldDuration = 10f;

    [Header("Shield Visual")]
    public SpriteRenderer[] shieldAffectedRenderers;

    [SerializeField] private Collider2D playerCollider;
    public System.Action<float> OnShieldCountdown;

    float shieldTimer = 0f; // ⭐ NEW: Track remaining shield time

    // ================= REVIVE SHIELD =================
    [Header("Revive Shield")]
    [SerializeField] private float reviveShieldDuration = 5f;
    private bool isReviveShieldActive = false;

    // ================= LAYERS =================
    int playerLayer;
    int obstacleLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (playerCollider == null)
            playerCollider = GetComponent<Collider2D>();

        playerLayer = LayerMask.NameToLayer("Player");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");

        if (playerLayer == -1 || obstacleLayer == -1)
            Debug.LogWarning("Player or Obstacle layer not found! Check Layer names.");
    }

    void Start()
    {
        ResetPhysics();
    }

    void FixedUpdate()
    {
        if (!lanesCalculated)
        {
            if (Camera.main == null) return;
            CalculateLanePositions();
            SnapToLane();
            lanesCalculated = true;
        }

        float targetSpeed = isBoosting ? boostSpeed : normalSpeed;

        if (forwardSpeed < targetSpeed)
        {
            float t = Mathf.InverseLerp(baseSpeed, targetSpeed, forwardSpeed);
            float accel = Mathf.Lerp(accelerationRate * 2.5f, accelerationRate * 0.4f, t);
            forwardSpeed += accel * Time.fixedDeltaTime;
        }
        else
        {
            float t = Mathf.InverseLerp(targetSpeed, boostSpeed, forwardSpeed);
            float decel = Mathf.Lerp(decelerationRate * 2.5f, decelerationRate * 0.5f, t);
            forwardSpeed -= decel * Time.fixedDeltaTime;
        }

        forwardSpeed = Mathf.Clamp(forwardSpeed, baseSpeed, boostSpeed);

        float targetX = lanePositions[currentLane];
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref smoothXVelocity, laneSmoothTime);
        float xVel = (newX - transform.position.x) / Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(xVel, forwardSpeed);

        if (carVisual != null)
        {
            float currentZ = carVisual.localEulerAngles.z;
            if (currentZ > 180f) currentZ -= 360f;

            float newZ = Mathf.Lerp(currentZ, targetRotationZ, Time.fixedDeltaTime * rotationSmoothSpeed);
            carVisual.localRotation = Quaternion.Euler(0f, 0f, newZ);

            if (Mathf.Abs(transform.position.x - targetX) < 0.05f)
                targetRotationZ = 0f;
        }
    }

    void Update()
    {
        if (isDead) return;
        HandleTouch();

        float targetDisplayedSpeed = forwardSpeed * 3.6f;
        displayedSpeed = Mathf.MoveTowards(displayedSpeed, targetDisplayedSpeed, Time.deltaTime * 50f);
    }

    // ================= SHIELD =================
    public void ActivateShield()
    {
        if (isShieldActive)
        {
            // ⭐ If shield already active, reset duration
            shieldTimer = totalShieldDuration;
            return;
        }

        StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        isShieldActive = true;
        shieldTimer = totalShieldDuration;

        SetTransparent(true);

        if (playerLayer != -1 && obstacleLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, true);

        while (shieldTimer > 0)
        {
            OnShieldCountdown?.Invoke(shieldTimer);
            shieldTimer -= Time.deltaTime;
            yield return null;
        }

        isShieldActive = false;
        SetTransparent(false);

        if (!isReviveShieldActive && playerLayer != -1 && obstacleLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, false);

        OnShieldCountdown?.Invoke(0);
    }

    public void ActivateReviveShield()
    {
        if (isReviveShieldActive) return;
        StartCoroutine(ReviveShieldRoutine());
    }

    IEnumerator ReviveShieldRoutine()
    {
        isReviveShieldActive = true;
        SetTransparent(true);

        if (playerLayer != -1 && obstacleLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, true);

        yield return new WaitForSeconds(reviveShieldDuration);

        SetTransparent(false);

        if (!isShieldActive && playerLayer != -1 && obstacleLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, false);

        isReviveShieldActive = false;
    }

    void SetTransparent(bool t)
    {
        float a = t ? 0.18f : 1f;
        foreach (var sr in shieldAffectedRenderers)
        {
            if (!sr) continue;
            var c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }

    // ================= RESET =================
    public void ResetPhysics()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        forwardSpeed = normalSpeed;
        displayedSpeed = 0f;

        isBoosting = false;
        swipeOnCooldown = false;
        smoothXVelocity = 0f;
        lanesCalculated = false;

        isDead = false;
        isShieldActive = false;
        isReviveShieldActive = false;

        targetRotationZ = 0f;
        if (carVisual) carVisual.localRotation = Quaternion.identity;

        if (playerLayer != -1 && obstacleLayer != -1)
            Physics2D.IgnoreLayerCollision(playerLayer, obstacleLayer, false);
    }

    // ================= INPUT =================
    void HandleTouch()
    {
        if (Input.touchCount == 0)
        {
            StopBoost();
            swipeOnCooldown = false;
            return;
        }

        Touch t = Input.GetTouch(0);
        if (t.phase == TouchPhase.Began)
        {
            startTouchPos = t.position;
            StartBoost();
        }
        else if (t.phase == TouchPhase.Moved && !swipeOnCooldown)
        {
            Vector2 d = t.position - startTouchPos;
            if (Mathf.Abs(d.x) > minSwipeDistance)
            {
                if (d.x > 0) MoveRight();
                else MoveLeft();

                startTouchPos = t.position;
                StartCoroutine(SwipeCooldown());
            }
        }
        else if (t.phase == TouchPhase.Ended)
        {
            StopBoost();
            swipeOnCooldown = false;
        }
    }

    void StartBoost()
    {
        isBoosting = true;
        carSoundController?.StartAccelerate();
    }

    void StopBoost()
    {
        isBoosting = false;
        carSoundController?.StopAccelerate();
    }

    IEnumerator SwipeCooldown()
    {
        swipeOnCooldown = true;
        yield return new WaitForSeconds(swipeCooldownTime);
        swipeOnCooldown = false;
    }

    void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            targetRotationZ = +maxTiltAngle;
        }
    }

    void MoveRight()
    {
        if (currentLane < 2)
        {
            currentLane++;
            targetRotationZ = -maxTiltAngle;
        }
    }

    // ================= COLLISION =================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (isShieldActive || isReviveShieldActive)
                return;

            if (!isDead)
            {
                isDead = true;
                audioSource?.PlayOneShot(deadSound);
                GameManager.Instance.GameOver();
            }
        }

        if (other.CompareTag("Fuel") || other.CompareTag("Coin"))
            audioSource?.PlayOneShot(collectSound);
    }

    // ================= LANE HELPERS =================
    void CalculateLanePositions()
    {
        Camera cam = Camera.main;
        float halfWidth = cam.orthographicSize * cam.aspect;

        lanePositions[0] = -halfWidth * laneScreenPadding;
        lanePositions[1] = 0f;
        lanePositions[2] = halfWidth * laneScreenPadding;
    }

    void SnapToLane()
    {
        Vector3 p = transform.position;
        p.x = lanePositions[currentLane];
        transform.position = p;
        rb.position = p;
    }

    // ================= BRAKE =================
public void ApplyBrake()
{
    StopAllCoroutines();
    StartCoroutine(BrakeRoutine());
}

IEnumerator BrakeRoutine()
{
    isBoosting = false; // boost band

    float brakeSpeed = forwardSpeed;

    while (brakeSpeed > normalSpeed)
    {
        brakeSpeed = Mathf.MoveTowards(
            brakeSpeed,
            normalSpeed,
            Time.deltaTime * 15f   // 🔥 brake power (adjustable)
        );

        forwardSpeed = brakeSpeed;
        yield return null;
    }

    forwardSpeed = normalSpeed;
}

}
