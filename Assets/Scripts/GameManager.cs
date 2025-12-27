using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;

    [Header("Revive UI")]
    public GameObject revivePanel;
    public TMP_Text reviveTimerText;
    public float reviveCountdownTime = 5f;

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isReviving = false;

    public event Action OnGameOver;
    public event Action OnGameRestart;

    private PlayerController player;
    private Vector3 savedPosition;
    private int savedLane;

    Coroutine reviveCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;

        player = FindFirstObjectByType<PlayerController>();

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (revivePanel) revivePanel.SetActive(false);
    }

    public void SavePlayerState()
    {
        if (!player) return;
        savedPosition = player.transform.position;
        savedLane = player.currentLane;
    }

    // ================= GAME OVER ENTRY =================
    public void GameOver()
    {
        if (isGameOver || isReviving) return;

        SavePlayerState();
        isReviving = true;

        Time.timeScale = 0f;

        if (revivePanel)
            revivePanel.SetActive(true);

        reviveCoroutine = StartCoroutine(ReviveCountdown());

        Debug.Log("PLAYER OUT → REVIVE OFFER");
    }

    // ================= REVIVE TIMER =================
    IEnumerator ReviveCountdown()
    {
        float t = reviveCountdownTime;

        while (t > 0)
        {
            if (reviveTimerText)
                reviveTimerText.text = Mathf.CeilToInt(t).ToString();

            yield return new WaitForSecondsRealtime(1f);
            t--;
        }

        // ❌ No revive → GAME OVER
        FinalGameOver();
    }

    // ================= CALLED FROM AD =================
    public void RevivePlayer()
    {
        if (!player) return;

        if (reviveCoroutine != null)
            StopCoroutine(reviveCoroutine);

        isReviving = false;
        isGameOver = false;

        if (revivePanel) revivePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        player.isDead = false;
        player.transform.position = savedPosition;
        player.currentLane = savedLane;
        player.ResetPhysics();
        player.ActivateReviveShield();

        Time.timeScale = 1f;

        MusicManager.Instance?.ResumeMusic();
        FindFirstObjectByType<CarSound>()?.ResumeSound();

        Debug.Log("PLAYER REVIVED ✅");
    }

    // ================= FINAL GAME OVER =================
    void FinalGameOver()
    {
        isReviving = false;
        isGameOver = true;

        if (revivePanel) revivePanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // 👉 Game over stats display karo (coins + score)
        Debug.Log("[GameManager] Looking for GameOverStats...");
        GameOverStats gameOverStats = FindObjectOfType<GameOverStats>();
        
        if (gameOverStats != null)
        {
            Debug.Log("[GameManager] GameOverStats found! Calling ShowGameOverStats()");
            gameOverStats.ShowGameOverStats();
        }
        else
        {
            Debug.LogError("[GameManager] GameOverStats NOT FOUND! Make sure GameOverStats script is attached to Game Over Panel!");
        }

        InterstitialAdController.Instance?.TryShowInterstitial();

        FuelManager.Instance?.HideFuelTemporarily();
        FindFirstObjectByType<Buttons>()?.HidePauseButton();
        MusicManager.Instance?.PauseMusicForGameOver();

        Debug.Log("GAME OVER ❌");
    }

    // ================= RESTART =================
    public void RestartGame()
    {
        // 👉 Restart se pehle coins aur score reset karo
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.ResetCoins();
        }

        SimpleScoreUI scoreUI = FindObjectOfType<SimpleScoreUI>();
        if (scoreUI != null)
        {
            scoreUI.ResetScore();
        }

        Time.timeScale = 1f;
        isGameOver = false;
        SceneManager.LoadScene("GamePlay");
        OnGameRestart?.Invoke();
    }
}
