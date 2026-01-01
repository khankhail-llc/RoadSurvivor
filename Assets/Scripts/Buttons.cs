
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class Buttons : MonoBehaviour
// {
//     [Header("Main Buttons")]
//     public Button playButton;
//     public Button restartButton;
//     public Button exitButton;
//     public Button garageButton;   // ✅ NEW

//     [Header("Pause UI")]
//     public Button pauseButton;
//     public GameObject pausedPanel;
//     public Button resumeButton;
//     public Button homeButton;

//     [Header("Settings UI")]
//     public Button settingsButton;
//     public GameObject settingsPanel;
//     public Button closeSettingsButton;

//     [Header("Scene Settings")]
//     public string gamePlaySceneName = "GamePlay";
//     public string mainMenuSceneName = "Main Menu";
//     public string garageSceneName = "Garage";   // ✅ NEW

//     private void Start()
//     {
//         playButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); StartGame(); });
//         restartButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); RestartGame(); });
//         exitButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); QuitGame(); });

//         garageButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); OpenGarage(); }); // ✅ NEW

//         pauseButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); PauseGame(); });
//         resumeButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); ResumeGame(); });
//         homeButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); GoHome(); });

//         settingsButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); OpenSettings(); });
//         closeSettingsButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); SettingsBack(); });

//         pausedPanel?.SetActive(false);
//         settingsPanel?.SetActive(false);

//         ShowPauseButton();
//     }

//     public void StartGame()
//     {
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(gamePlaySceneName);
//     }

//     public void RestartGame()
//     {
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     }

//     public void OpenGarage()   // ✅ GARAGE FUNCTION
//     {
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(garageSceneName);
//     }

//     public void QuitGame()
//     {
// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
//     }

//     public void PauseGame()
//     {
//         Time.timeScale = 0f;
//         pausedPanel?.SetActive(true);
//     }

//     public void ResumeGame()
//     {
//         Time.timeScale = 1f;
//         pausedPanel?.SetActive(false);
//     }

//     public void GoHome()
//     {
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(mainMenuSceneName);
//     }

//     public void OpenSettings()
//     {
//         settingsPanel?.SetActive(true);
//         Time.timeScale = 0f;
//     }

//     public void SettingsBack()
//     {
//         settingsPanel?.SetActive(false);
//         Time.timeScale = 1f;
//     }

//     public void HidePauseButton() => pauseButton?.gameObject.SetActive(false);
//     public void ShowPauseButton() => pauseButton?.gameObject.SetActive(true);
// }





using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    [Header("Main Buttons")]
    public Button playButton;
    public Button restartButton;
    public Button exitButton;
    public Button garageButton;

    // 👉 NEW MAIN MENU BUTTON
    public Button mainMenuButton;

    [Header("More Games")]
    public Button moreGamesButton;
    public string moreGamesUrl = "https://play.google.com/store/apps/dev?id=YOUR_DEV_ID"; // Replace with actual URL

    [Header("Pause UI")]
    public Button pauseButton;
    public GameObject pausedPanel;
    public Button resumeButton;
    public Button homeButton;

    [Header("Settings UI")]
    public Button settingsButton;
    public GameObject settingsPanel;
    public Button closeSettingsButton;

    [Header("Scene Settings")]
    public string gamePlaySceneName = "GamePlay";
    public string mainMenuSceneName = "Main Menu";
    public string garageSceneName = "Garage";

    private void Start()
    {
        playButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); StartGame(); });
        restartButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); RestartGame(); });
        exitButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); QuitGame(); });
        
        garageButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); OpenGarage(); });
        
        moreGamesButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); OpenMoreGames(); });

        // 👉 MAIN MENU BUTTON LISTENER
        mainMenuButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); GoHome(); });

        pauseButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); PauseGame(); });
        resumeButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); ResumeGame(); });
        homeButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); GoHome(); });

        settingsButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); OpenSettings(); });
        closeSettingsButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); SettingsBack(); });

        pausedPanel?.SetActive(false);
        settingsPanel?.SetActive(false);

        ShowPauseButton();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenGarage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(garageSceneName);
    }

    public void OpenMoreGames()
    {
        Application.OpenURL(moreGamesUrl);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausedPanel?.SetActive(true);

        MusicManager.Instance?.PauseMusicByUser();
        FindFirstObjectByType<CarSound>()?.PauseSoundByUser();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausedPanel?.SetActive(false);

        MusicManager.Instance?.ResumeMusicByUser();
        FindFirstObjectByType<CarSound>()?.ResumeSoundByUser();
    }

    // 👉 MAIN MENU FUNCTION
    public void GoHome()
    {
        // 👉 Home jate waqt coins aur score reset karo
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
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);
        ToggleButtons(false);
        Time.timeScale = 0f;
    }

    public void SettingsBack()
    {
        settingsPanel?.SetActive(false);
        ToggleButtons(true);
        Time.timeScale = 1f;
    }

    private void ToggleButtons(bool state)
    {
        if (playButton) playButton.interactable = state;
        if (garageButton) garageButton.interactable = state;
        if (exitButton) exitButton.interactable = state;
        if (moreGamesButton) moreGamesButton.interactable = state;
        if (mainMenuButton) mainMenuButton.interactable = state;
    }

    public void HidePauseButton() => pauseButton?.gameObject.SetActive(false);
    public void ShowPauseButton() => pauseButton?.gameObject.SetActive(true);
}
