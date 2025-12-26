using UnityEngine;
using System.Collections;

public class ButtonSoundVolume : MonoBehaviour
{
    [Header("Button Click Sound")]
    public AudioSource buttonAudioSource;

    [Header("Popup Animation")]
    public GameObject popup;
    public float animationTime = 0.25f;

    private bool isMuted = false;
    private float defaultVolume = 1f;

    void Start()
    {
        if (buttonAudioSource != null)
            defaultVolume = buttonAudioSource.volume;

        if (popup != null)
        {
            popup.SetActive(false);
            popup.transform.localScale = Vector3.zero;
        }
    }

    // Button OnClick() se call karo
    public void ToggleButtonSound()
    {
        if (buttonAudioSource == null) return;

        isMuted = !isMuted;

        if (isMuted)
            buttonAudioSource.volume = 0f;            // OFF
        else
            buttonAudioSource.volume = defaultVolume; // ON

        if (popup != null)
            StartCoroutine(PopupAnimation());
    }

    IEnumerator PopupAnimation()
    {
        popup.SetActive(true);

        float t = 0f;

        // SCALE UP
        while (t < animationTime)
        {
            t += Time.unscaledDeltaTime;
            popup.transform.localScale =
                Vector3.Lerp(Vector3.zero, Vector3.one, t / animationTime);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.4f);

        t = 0f;

        // SCALE DOWN
        while (t < animationTime)
        {
            t += Time.unscaledDeltaTime;
            popup.transform.localScale =
                Vector3.Lerp(Vector3.one, Vector3.zero, t / animationTime);
            yield return null;
        }

        popup.SetActive(false);
    }
}
