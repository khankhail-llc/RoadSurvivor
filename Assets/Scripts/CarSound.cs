using UnityEngine;

public class CarSound : MonoBehaviour
{
    public AudioSource engineSource;

    [Header("Sound Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float minVolume = 0.3f;
    public float maxVolume = 1.0f;

    [Header("Player")]
    public Rigidbody2D playerRB;

    private bool isGameOver = false;

    void Start()
    {
        engineSource.loop = true;
        engineSource.pitch = minPitch;
        engineSource.volume = minVolume;
        engineSource.Play();
    }

    void Update()
    {
        if (isGameOver) return;

        float speed = playerRB.linearVelocity.y;

        float t = Mathf.Clamp01(speed / 15f);

        float targetPitch = Mathf.Lerp(minPitch, maxPitch, t);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, t);

        float globalVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * 3f);
        engineSource.volume = Mathf.Lerp(engineSource.volume, targetVolume * globalVolume, Time.deltaTime * 3f);

        // Explicit mute if global volume is 0
        if (globalVolume <= 0.01f)
            engineSource.volume = 0;
    }

    public void PlayerOut()
    {
        isGameOver = true;
        engineSource.Stop();
    }

    public void PauseSoundByUser()
    {
        if (engineSource.isPlaying)
        {
            engineSource.Pause();
        }
    }

    public void ResumeSoundByUser()
    {
        if (!isGameOver)
        {
            engineSource.UnPause();
        }
    }

    // 🔥 NEW METHOD TO RESUME SOUND
    public void ResumeSound()
    {
        isGameOver = false;

        // If it was paused, UnPause; otherwise Play
        engineSource.UnPause();
        if (!engineSource.isPlaying)
        {
            engineSource.Play();
        }
    }
}
