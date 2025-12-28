using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    [Header("GamePlay Scene Name")]
    public string gamePlaySceneName = "GamePlay";

    [Header("Music Settings")]
    public Slider musicSlider;

    [HideInInspector] public bool isPausedByAd = false;

    private bool isPausedByUser = false;
    private bool wasPlayingBeforePause = false;
    private float savedMusicTime = 0f;
    private bool wasPlayingBeforeGameOver = false;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ================= SCENE CONTROL =================

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadMusicState();

        if (scene.name == gamePlaySceneName)
        {
            if (!isPausedByAd && audioSource.volume > 0)
                audioSource.Play();
        }
        else
        {
            // ❌ Main Menu ya koi aur scene → music band
            audioSource.Stop();
        }
    }

    // ================= SLIDER REGISTER =================

    public void RegisterSlider(Slider newSlider)
    {
        musicSlider = newSlider;
        if (musicSlider == null) return;

        musicSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        float volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        musicSlider.value = volume;
        SetVolume(volume);
    }

    // ================= SLIDER CHANGE =================

    void OnMusicSliderChanged(float value)
    {
        SetVolume(value);

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        PlayerPrefs.Save();

        // 🔒 IMPORTANT: sirf GamePlay scene mein play ho
        if (value > 0.001f &&
            !audioSource.isPlaying &&
            !isPausedByAd &&
            SceneManager.GetActiveScene().name == gamePlaySceneName)
        {
            audioSource.Play();
        }
    }

    void LoadMusicState()
    {
        float volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        SetVolume(volume);

        if (musicSlider != null)
            musicSlider.value = volume;
    }

    // ================= AD CONTROL =================

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            isPausedByAd = true;
            audioSource.Pause();
        }
    }

    public void ResumeMusicAfterAd()
    {
        if (isPausedByAd && audioSource.volume > 0 &&
            SceneManager.GetActiveScene().name == gamePlaySceneName)
        {
            audioSource.UnPause();
            isPausedByAd = false;
        }
    }

    // ================= USER CONTROL =================

    public void PauseMusicByUser()
    {
        if (audioSource.isPlaying)
        {
            wasPlayingBeforePause = true;
            audioSource.Pause();
            isPausedByUser = true;
        }
    }

    public void ResumeMusicByUser()
    {
        if (isPausedByUser && wasPlayingBeforePause &&
            audioSource.volume > 0 &&
            SceneManager.GetActiveScene().name == gamePlaySceneName)
        {
            audioSource.UnPause();
            isPausedByUser = false;
            wasPlayingBeforePause = false;
        }
    }

    // ================= GAME OVER =================

    public void PauseMusicForGameOver()
    {
        if (audioSource.isPlaying)
        {
            savedMusicTime = audioSource.time;
            wasPlayingBeforeGameOver = true;
            audioSource.Stop();
        }
    }

    public void ResumeMusic()
    {
        if (wasPlayingBeforeGameOver &&
            audioSource.volume > 0 &&
            !isPausedByAd &&
            SceneManager.GetActiveScene().name == gamePlaySceneName)
        {
            audioSource.time = savedMusicTime;
            audioSource.Play();
            wasPlayingBeforeGameOver = false;
        }
    }

    // ================= VOLUME =================

    public void SetVolume(float value)
    {
        audioSource.volume = value;

        if (value <= 0.001f)
        {
            audioSource.mute = true;
            audioSource.Stop();
        }
        else
        {
            audioSource.mute = false;
            // ❌ yahan Play NA karo
        }
    }

    public void RestartMusic()
    {
        if (audioSource.volume <= 0.001f) return;
        if (SceneManager.GetActiveScene().name != gamePlaySceneName) return;

        audioSource.Stop();
        audioSource.time = 0;
        audioSource.Play();
    }
}
