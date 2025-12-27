using UnityEngine;
public class ClickSound : MonoBehaviour
{
    public static ClickSound Instance;
    private AudioSource audioSource;

    void Awake()
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
    }

    public void PlayClick()
    {
        audioSource.Play();
    }
}
