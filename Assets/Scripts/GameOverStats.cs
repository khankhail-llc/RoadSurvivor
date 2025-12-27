using UnityEngine;
using TMPro;

/// <summary>
/// Game Over Panel mein current game ka coin aur score dikhata hai
/// UPDATED VERSION - Public getters use karta hai
/// </summary>
public class GameOverStats : MonoBehaviour
{
    [Header("Game Over Panel Text Fields")]
    [SerializeField] private TextMeshProUGUI gameOverCoinText;    // "COIN : X" text
    [SerializeField] private TextMeshProUGUI gameOverScoreText;   // "TOTAL SCORE : X" text

    /// <summary>
    /// Game over panel show hone par ye function call karo
    /// Current coins aur score ko game over panel mein update karega
    /// </summary>
    public void ShowGameOverStats()
    {
        // Current game ke coins get karo
        int currentCoins = 0;
        if (CoinManager.Instance != null)
        {
            currentCoins = CoinManager.Instance.GetCurrentCoins();
            Debug.Log($"[GameOverStats] Current coins: {currentCoins}");
        }
        else
        {
            Debug.LogWarning("[GameOverStats] CoinManager.Instance is null!");
        }

        // Current game ka score get karo
        int currentScore = 0;
        SimpleScoreUI scoreUI = FindObjectOfType<SimpleScoreUI>();
        if (scoreUI != null)
        {
            currentScore = scoreUI.GetCurrentScore();
            Debug.Log($"[GameOverStats] Current score: {currentScore}");
        }
        else
        {
            Debug.LogWarning("[GameOverStats] SimpleScoreUI not found!");
        }

        // Game over panel mein display karo
        if (gameOverCoinText != null)
        {
            gameOverCoinText.text = currentCoins.ToString();
            Debug.Log($"[GameOverStats] Set coin text to: {currentCoins}");
        }
        else
        {
            Debug.LogError("[GameOverStats] gameOverCoinText is not assigned in Inspector!");
        }

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = currentScore.ToString();
            Debug.Log($"[GameOverStats] Set score text to: {currentScore}");
        }
        else
        {
            Debug.LogError("[GameOverStats] gameOverScoreText is not assigned in Inspector!");
        }

        Debug.Log($"[GameOverStats] Game Over Stats Updated - Coins: {currentCoins}, Score: {currentScore}");
    }
}
