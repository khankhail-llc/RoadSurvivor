using UnityEngine;
using TMPro;

public class SimpleScoreUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Score Settings")]
    [SerializeField] float countSpeed = 10f; // score per second

    [Header("Pop Animation")]
    [SerializeField] float popScale = 1.2f;
    [SerializeField] float popSpeed = 8f;

    private int score;
    private float timer;
    private Vector3 originalScale;

    private const string BEST_SCORE_KEY = "BEST_SCORE";

    void Start()
    {
        score = 0;
        scoreText.text = "00";
        originalScale = scoreText.transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime * countSpeed;

        if (timer >= 1f)
        {
            timer = 0f;
            score++;
            UpdateScoreText();
            PlayPopAnimation();
        }
    }

    // ---------------- SCORE DISPLAY ----------------
    void UpdateScoreText()
    {
        if (score < 100)
            scoreText.text = score.ToString("00");
        else
            scoreText.text = score.ToString();
    }

    // ---------------- POP ANIMATION ----------------
    void PlayPopAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(PopRoutine());
    }

    System.Collections.IEnumerator PopRoutine()
    {
        // scale up
        scoreText.transform.localScale = originalScale * popScale;
        yield return new WaitForSeconds(0.05f);

        // smooth scale back
        while (Vector3.Distance(scoreText.transform.localScale, originalScale) > 0.01f)
        {
            scoreText.transform.localScale = Vector3.Lerp(
                scoreText.transform.localScale,
                originalScale,
                Time.deltaTime * popSpeed
            );
            yield return null;
        }

        scoreText.transform.localScale = originalScale;
    }

    // ---------------- BEST SCORE ----------------
    public void SaveBestScore()
    {
        int best = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);

        if (score > best)
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, score);
            PlayerPrefs.Save();
        }
    }

    public int GetBestScore()
    {
        return PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
    }

    // ---------------- GAME CONTROL ----------------
    public void ResetScore()
    {
        score = 0;
        timer = 0f;
        UpdateScoreText();
    }

    public void StopScore()
    {
        SaveBestScore();
        enabled = false;
    }
}
