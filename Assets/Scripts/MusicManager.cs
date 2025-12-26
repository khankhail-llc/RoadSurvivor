using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    [Header("GamePlay Scene Name")]
    public string gamePlaySceneName = "GamePlay";

    [HideInInspector] public bool isPausedByAd = false;
    private bool isPausedByUser = false; // User ne pause button se pause kiya hai
    private bool wasPlayingBeforePause = false; // Pause se pehle playing thi ya nahi
    private float savedMusicTime = 0f; // Game over se pehle music ka time save karo
    private bool wasPlayingBeforeGameOver = false; // Game over se pehle playing thi ya nahi

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
        if (audioSource == null) Debug.LogError("MusicManager: No AudioSource found!");
        audioSource.loop = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null) return;

        if (scene.name == gamePlaySceneName)
        {
            if (!isPausedByAd && !audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (!isPausedByAd)
                audioSource.Stop();
        }
    }

    public void PlayMusic()
    {
        if (audioSource == null) return;
        if (!audioSource.isPlaying) audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource == null) return;
        // Complete stop (restart ke liye)
        if (!isPausedByAd)
        {
            audioSource.Stop();
            isPausedByUser = false;
            savedMusicTime = 0f;
            wasPlayingBeforeGameOver = false;
        }
    }
    
    // Game over par stop karo lekin time save karo (rewarded ad ke baad resume karne ke liye)
    public void PauseMusicForGameOver()
    {
        if (audioSource == null) return;
        
        // Agar paused hai to pehle unpause karo time get karne ke liye
        if (!audioSource.isPlaying && audioSource.time > 0)
        {
            audioSource.UnPause();
        }
        
        // Current time save karo (agar playing hai ya paused hai)
        if (audioSource.time > 0)
        {
            savedMusicTime = audioSource.time;
            wasPlayingBeforeGameOver = true;
        }
        
        // Forcefully stop karo (chahe playing ho ya paused)
        audioSource.Stop();
        
        // Reset flags
        isPausedByAd = false;
        isPausedByUser = false;
    }

    public void PauseMusic()
    {
        if (audioSource == null) return;
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPausedByAd = true;
        }
    }

    public void ResumeMusic()
    {
        if (audioSource == null) return;
        // Agar ad ne pause kiya tha to resume karo
        if (isPausedByAd)
        {
            audioSource.UnPause();
            isPausedByAd = false;
        }
        // Agar game over se pehle playing thi to wahi se resume karo
        else if (wasPlayingBeforeGameOver)
        {
            audioSource.time = savedMusicTime; // Wahi time set karo
            audioSource.Play(); // Play karo
            wasPlayingBeforeGameOver = false;
            savedMusicTime = 0f;
        }
    }
    
    // User pause button ke liye (pause menu)
    public void PauseMusicByUser()
    {
        if (audioSource == null) return;
        
        // Agar playing hai to pause karo
        if (audioSource.isPlaying)
        {
            wasPlayingBeforePause = true;
            audioSource.Pause();
            isPausedByUser = true;
            
            // Ensure pause ho gaya hai
            if (audioSource.isPlaying)
            {
                // Agar pause nahi hua to forcefully stop karo
                audioSource.Stop();
            }
        }
        // Agar already paused hai (by ad) to bhi flag set karo
        else if (audioSource.time > 0 && !audioSource.isPlaying)
        {
            wasPlayingBeforePause = true;
            isPausedByUser = true;
            // Already paused hai, kuch nahi karna
        }
    }
    
    // User resume button ke liye (pause menu)
    public void ResumeMusicByUser()
    {
        if (audioSource == null) return;
        if (isPausedByUser && wasPlayingBeforePause)
        {
            audioSource.UnPause();
            isPausedByUser = false;
            wasPlayingBeforePause = false;
        }
    }

    public void SetVolume(float value)
    {
        if (audioSource == null) return;
        audioSource.volume = value;
    }

    public void RestartMusic()
    {
        if (audioSource == null) return;
        audioSource.Stop();
        audioSource.time = 0;
        audioSource.Play();
    }
}
