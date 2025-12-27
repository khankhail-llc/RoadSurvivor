// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// public class VolumeButton : MonoBehaviour
// {
//     [Header("Button Settings")]
//     public Button volumeButton;        // Drag your UI Button here
//     public float clickScale = 1.2f;    // Scale on click
//     public float scaleSpeed = 0.1f;    // How fast it animates back

//     [Header("Audio Settings")]
//     public AudioSource[] allAudioSources; // All AudioSources in game

//     private bool isMuted = false;
//     private Vector3 originalScale;

//     private const string MutePrefKey = "GameMuted";

//     void Awake()
//     {
//         // Load saved mute state
//         isMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;

//         // Update all AudioSources at start
//         if (allAudioSources == null || allAudioSources.Length == 0)
//             allAudioSources = FindObjectsOfType<AudioSource>();

//         UpdateAllAudioSources();

//         if (volumeButton == null)
//             volumeButton = GetComponent<Button>();

//         originalScale = volumeButton.transform.localScale;
//         volumeButton.onClick.AddListener(OnButtonClick);
//     }

//     void OnButtonClick()
//     {
//         // Toggle mute state
//         isMuted = !isMuted;

//         // Save state
//         PlayerPrefs.SetInt(MutePrefKey, isMuted ? 1 : 0);
//         PlayerPrefs.Save();

//         UpdateAllAudioSources();

//         // Animate button
//         StopAllCoroutines();
//         StartCoroutine(ClickAnimation());
//     }

//     void UpdateAllAudioSources()
//     {
//         foreach (AudioSource a in allAudioSources)
//         {
//             if (a != null)
//                 a.mute = isMuted;
//         }
//     }

//     IEnumerator ClickAnimation()
//     {
//         Vector3 targetScale = originalScale * clickScale;
//         float t = 0f;

//         // Scale up
//         while (t < 1f)
//         {
//             t += Time.deltaTime / scaleSpeed;
//             volumeButton.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
//             yield return null;
//         }

//         t = 0f;

//         // Scale back
//         while (t < 1f)
//         {
//             t += Time.deltaTime / scaleSpeed;
//             volumeButton.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
//             yield return null;
//         }

//         volumeButton.transform.localScale = originalScale;
//     }

//     // Call this when a new AudioSource is created at runtime
//     public void RegisterAudioSource(AudioSource source)
//     {
//         if (allAudioSources == null) allAudioSources = new AudioSource[0];

//         var list = new System.Collections.Generic.List<AudioSource>(allAudioSources);
//         if (!list.Contains(source))
//             list.Add(source);

//         allAudioSources = list.ToArray();

//         // Apply current mute state
//         if (source != null)
//             source.mute = isMuted;
//     }
// }




using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VolumeButton : MonoBehaviour
{
    [Header("Button Settings")]
    public Button volumeButton;
    public float clickScale = 1.2f;
    public float scaleSpeed = 0.1f;

    [Header("Audio Settings")]
    public AudioSource[] allAudioSources;

    [Header("Slider Settings")]
    public Slider volumeSlider;              // 👉 Yahan apna Slider drag karo
    public AudioSource musicSource;           // 👉 Music Manager ka AudioSource

    private bool isMuted = false;
    private Vector3 originalScale;

    private const string MutePrefKey = "GameMuted";
    private const string VolumePrefKey = "GameVolume";

    void Awake()
    {
        // Load saved values
        isMuted = PlayerPrefs.GetInt(MutePrefKey, 0) == 1;
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);

        if (allAudioSources == null || allAudioSources.Length == 0)
            allAudioSources = FindObjectsOfType<AudioSource>();

        UpdateAllAudioSources();

        if (volumeButton == null)
            volumeButton = GetComponent<Button>();

        originalScale = volumeButton.transform.localScale;
        volumeButton.onClick.AddListener(OnButtonClick);

        // Slider setup
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        ApplyVolume(savedVolume);
    }

    void OnButtonClick()
    {
        isMuted = !isMuted;

        PlayerPrefs.SetInt(MutePrefKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateAllAudioSources();

        StopAllCoroutines();
        StartCoroutine(ClickAnimation());
    }

    void OnSliderValueChanged(float value)
    {
        ApplyVolume(value);

        // Save volume
        PlayerPrefs.SetFloat(VolumePrefKey, value);
        PlayerPrefs.Save();

        // Auto mute logic
        isMuted = value <= 0.01f;
        PlayerPrefs.SetInt(MutePrefKey, isMuted ? 1 : 0);
    }

    void ApplyVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            musicSource.mute = volume <= 0.01f;
        }
    }

    void UpdateAllAudioSources()
    {
        foreach (AudioSource a in allAudioSources)
        {
            if (a != null)
                a.mute = isMuted;
        }
    }

    IEnumerator ClickAnimation()
    {
        Vector3 targetScale = originalScale * clickScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / scaleSpeed;
            volumeButton.transform.localScale =
                Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / scaleSpeed;
            volumeButton.transform.localScale =
                Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        volumeButton.transform.localScale = originalScale;
    }

    public void RegisterAudioSource(AudioSource source)
    {
        if (allAudioSources == null) allAudioSources = new AudioSource[0];

        var list = new System.Collections.Generic.List<AudioSource>(allAudioSources);
        if (!list.Contains(source))
            list.Add(source);

        allAudioSources = list.ToArray();

        if (source != null)
            source.mute = isMuted;
    }
}
