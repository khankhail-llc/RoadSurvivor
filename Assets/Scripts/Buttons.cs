// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class Buttons : MonoBehaviour
// {
//     [Header("Main Buttons")]
//     public Button playButton;
//     public Button restartButton;
//     public Button exitButton;

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

//     private void Start()
//     {
//         playButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); StartGame(); });
//         restartButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); RestartGame(); });
//         exitButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); QuitGame(); });

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

//         GameManager.Instance?.ResetGame();
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);

//         BannerAdController.Instance?.ReloadBanner();
//         RewardedAdController.Instance?.LoadRewardedAd();
//         InterstitialAdController.Instance?.LoadInterstitial();
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
//         FuelManager.Instance?.HideFuelTemporarily();
//         MusicManager.Instance?.PauseMusicByUser();

//         var carSound = FindFirstObjectByType<CarSound>();
//         if (carSound != null && carSound.engineSource != null)
//             carSound.engineSource.Pause();

//         var carSoundController = FindFirstObjectByType<CarSoundController>();
//         if (carSoundController != null && carSoundController.engineSound != null)
//             carSoundController.engineSound.Pause();
//     }

//     public void ResumeGame()
//     {
//         Time.timeScale = 1f;
//         pausedPanel?.SetActive(false);
//         FuelManager.Instance?.ShowFuelAfterMenu();
//         MusicManager.Instance?.ResumeMusicByUser();

//         var carSound = FindFirstObjectByType<CarSound>();
//         if (carSound != null && carSound.engineSource != null)
//             carSound.engineSource.UnPause();

//         var carSoundController = FindFirstObjectByType<CarSoundController>();
//         if (carSoundController != null && carSoundController.engineSound != null)
//             carSoundController.engineSound.UnPause();
//     }

//     public void GoHome()
//     {
//         Time.timeScale = 1f;
//         SceneManager.LoadScene(mainMenuSceneName);
//     }

//     public void HidePauseButton() => pauseButton?.gameObject.SetActive(false);
//     public void ShowPauseButton() => pauseButton?.gameObject.SetActive(true);

//     public void OpenSettings()
//     {
//         settingsPanel?.SetActive(true);
//         Time.timeScale = 0f;
//         FuelManager.Instance?.HideFuelTemporarily();
//         MusicManager.Instance?.PauseMusicByUser();

//         var carSound = FindFirstObjectByType<CarSound>();
//         if (carSound != null && carSound.engineSource != null)
//             carSound.engineSource.Pause();

//         var carSoundController = FindFirstObjectByType<CarSoundController>();
//         if (carSoundController != null && carSoundController.engineSound != null)
//             carSoundController.engineSound.Pause();
//     }

//     public void SettingsBack()
//     {
//         settingsPanel.SetActive(false);
//         Time.timeScale = 1f;
//         FuelManager.Instance?.ShowFuelAfterMenu();
//         MusicManager.Instance?.ResumeMusicByUser();

//         var carSound = FindFirstObjectByType<CarSound>();
//         if (carSound != null && carSound.engineSource != null)
//             carSound.engineSource.UnPause();

//         var carSoundController = FindFirstObjectByType<CarSoundController>();
//         if (carSoundController != null && carSoundController.engineSound != null)
//             carSoundController.engineSound.UnPause();
//     }
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
    public Button garageButton;   // ✅ NEW

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
    public string garageSceneName = "Garage";   // ✅ NEW

    private void Start()
    {
        playButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); StartGame(); });
        restartButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); RestartGame(); });
        exitButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); QuitGame(); });

        garageButton?.onClick.AddListener(() => { ClickSound.Instance?.PlayClick(); OpenGarage(); }); // ✅ NEW

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

    public void OpenGarage()   // ✅ GARAGE FUNCTION
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(garageSceneName);
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
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausedPanel?.SetActive(false);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenSettings()
    {
        settingsPanel?.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SettingsBack()
    {
        settingsPanel?.SetActive(false);
        Time.timeScale = 1f;
    }

    public void HidePauseButton() => pauseButton?.gameObject.SetActive(false);
    public void ShowPauseButton() => pauseButton?.gameObject.SetActive(true);
}
