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
    private const string MUSIC_KEY = "MusicState"; // 1 = ON , 0 = OFF

    [HideInInspector] public bool isPausedByAd = false;

    private bool isPausedByUser = false;
    private bool wasPlayingBeforePause = false;
    private float savedMusicTime = 0f;
    private bool wasPlayingBeforeGameOver = false;

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

        LoadMusicState();
    }

    private void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null) return;

        if (scene.name == gamePlaySceneName)
        {
            if (!isPausedByAd && audioSource.volume > 0 && !audioSource.mute)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    // ================= MUSIC SAVE / LOAD =================

    void OnMusicSliderChanged(float value)
    {
        SetVolume(value);

        if (value <= 0.001f)
        {
            PlayerPrefs.SetInt(MUSIC_KEY, 0);
            audioSource.Stop();
        }
        else
        {
            PlayerPrefs.SetInt(MUSIC_KEY, 1);
            if (!audioSource.isPlaying && !isPausedByAd)
                audioSource.Play();
        }

        PlayerPrefs.Save();
    }

    void LoadMusicState()
    {
        int state = PlayerPrefs.GetInt(MUSIC_KEY, 1);

        if (musicSlider != null)
            musicSlider.value = state == 1 ? 1f : 0f;

        audioSource.volume = state == 1 ? 1f : 0f;
        audioSource.mute = state == 0;

        if (state == 0)
            audioSource.Stop();
    }

    // ================= AD PAUSE / RESUME =================

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            isPausedByAd = true;
            audioSource.Pause();
        }
    }

    public void ResumeMusicAfterAd()
    {
        if (audioSource != null && isPausedByAd && audioSource.volume > 0 && !audioSource.mute)
        {
            audioSource.UnPause();
            isPausedByAd = false;
        }
    }

    // ================= EXISTING FUNCTIONS =================

    public void PlayMusic()
    {
        if (audioSource.volume == 0 || audioSource.mute || isPausedByAd) return;
        if (!audioSource.isPlaying) audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
        savedMusicTime = 0f;
        wasPlayingBeforeGameOver = false;
    }

    public void PauseMusicForGameOver()
    {
        if (audioSource.time > 0)
        {
            savedMusicTime = audioSource.time;
            wasPlayingBeforeGameOver = true;
        }
        audioSource.Stop();
    }

    public void ResumeMusic()
    {
        if (audioSource.volume == 0 || audioSource.mute || isPausedByAd) return;

        if (wasPlayingBeforeGameOver)
        {
            audioSource.time = savedMusicTime;
            audioSource.Play();
            wasPlayingBeforeGameOver = false;
        }
    }

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
        if (isPausedByUser && wasPlayingBeforePause && !audioSource.mute)
        {
            audioSource.UnPause();
            isPausedByUser = false;
            wasPlayingBeforePause = false;
        }
    }

    // 🔥 MAIN FIX: HARD MUTE
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
        }
    }

    public void RestartMusic()
    {
        if (audioSource.volume == 0 || audioSource.mute) return;
        audioSource.Stop();
        audioSource.time = 0;
        audioSource.Play();
    }
}
