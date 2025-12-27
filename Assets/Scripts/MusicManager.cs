// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class MusicManager : MonoBehaviour
// {
//     public static MusicManager Instance;

//     private AudioSource audioSource;

//     [Header("GamePlay Scene Name")]
//     public string gamePlaySceneName = "GamePlay";

//     [HideInInspector] public bool isPausedByAd = false;
//     private bool isPausedByUser = false; // User ne pause button se pause kiya hai
//     private bool wasPlayingBeforePause = false; // Pause se pehle playing thi ya nahi
//     private float savedMusicTime = 0f; // Game over se pehle music ka time save karo
//     private bool wasPlayingBeforeGameOver = false; // Game over se pehle playing thi ya nahi

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//             return;
//         }

//         audioSource = GetComponent<AudioSource>();
//         if (audioSource == null) Debug.LogError("MusicManager: No AudioSource found!");
//         audioSource.loop = true;

//         SceneManager.sceneLoaded += OnSceneLoaded;
//     }

//     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         if (audioSource == null) return;

//         if (scene.name == gamePlaySceneName)
//         {
//             if (!isPausedByAd && !audioSource.isPlaying)
//                 audioSource.Play();
//         }
//         else
//         {
//             if (!isPausedByAd)
//                 audioSource.Stop();
//         }
//     }

//     public void PlayMusic()
//     {
//         if (audioSource == null) return;
//         if (!audioSource.isPlaying) audioSource.Play();
//     }

//     public void StopMusic()
//     {
//         if (audioSource == null) return;
//         // Complete stop (restart ke liye)
//         if (!isPausedByAd)
//         {
//             audioSource.Stop();
//             isPausedByUser = false;
//             savedMusicTime = 0f;
//             wasPlayingBeforeGameOver = false;
//         }
//     }
    
//     // Game over par stop karo lekin time save karo (rewarded ad ke baad resume karne ke liye)
//     public void PauseMusicForGameOver()
//     {
//         if (audioSource == null) return;
        
//         // Agar paused hai to pehle unpause karo time get karne ke liye
//         if (!audioSource.isPlaying && audioSource.time > 0)
//         {
//             audioSource.UnPause();
//         }
        
//         // Current time save karo (agar playing hai ya paused hai)
//         if (audioSource.time > 0)
//         {
//             savedMusicTime = audioSource.time;
//             wasPlayingBeforeGameOver = true;
//         }
        
//         // Forcefully stop karo (chahe playing ho ya paused)
//         audioSource.Stop();
        
//         // Reset flags
//         isPausedByAd = false;
//         isPausedByUser = false;
//     }

//     public void PauseMusic()
//     {
//         if (audioSource == null) return;
//         if (audioSource.isPlaying)
//         {
//             audioSource.Pause();
//             isPausedByAd = true;
//         }
//     }

//     public void ResumeMusic()
//     {
//         if (audioSource == null) return;
//         // Agar ad ne pause kiya tha to resume karo
//         if (isPausedByAd)
//         {
//             audioSource.UnPause();
//             isPausedByAd = false;
//         }
//         // Agar game over se pehle playing thi to wahi se resume karo
//         else if (wasPlayingBeforeGameOver)
//         {
//             audioSource.time = savedMusicTime; // Wahi time set karo
//             audioSource.Play(); // Play karo
//             wasPlayingBeforeGameOver = false;
//             savedMusicTime = 0f;
//         }
//     }
    
//     // User pause button ke liye (pause menu)
//     public void PauseMusicByUser()
//     {
//         if (audioSource == null) return;
        
//         // Agar playing hai to pause karo
//         if (audioSource.isPlaying)
//         {
//             wasPlayingBeforePause = true;
//             audioSource.Pause();
//             isPausedByUser = true;
            
//             // Ensure pause ho gaya hai
//             if (audioSource.isPlaying)
//             {
//                 // Agar pause nahi hua to forcefully stop karo
//                 audioSource.Stop();
//             }
//         }
//         // Agar already paused hai (by ad) to bhi flag set karo
//         else if (audioSource.time > 0 && !audioSource.isPlaying)
//         {
//             wasPlayingBeforePause = true;
//             isPausedByUser = true;
//             // Already paused hai, kuch nahi karna
//         }
//     }
    
//     // User resume button ke liye (pause menu)
//     public void ResumeMusicByUser()
//     {
//         if (audioSource == null) return;
//         if (isPausedByUser && wasPlayingBeforePause)
//         {
//             audioSource.UnPause();
//             isPausedByUser = false;
//             wasPlayingBeforePause = false;
//         }
//     }

//     public void SetVolume(float value)
//     {
//         if (audioSource == null) return;
//         audioSource.volume = value;
//     }

//     public void RestartMusic()
//     {
//         if (audioSource == null) return;
//         audioSource.Stop();
//         audioSource.time = 0;
//         audioSource.Play();
//     }
// }




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
