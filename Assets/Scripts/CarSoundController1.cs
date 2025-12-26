using UnityEngine;

public class CarSoundController : MonoBehaviour
{
    public AudioSource engineSound;

    [Header("Speed Settings")]
    public float normalPitch = 1f;
    public float maxPitch = 1.6f;
    public float accelerateSpeed = 2f; // speed when player taps

    private bool isAccelerating = false;

    void Update()
    {
        // Only modify pitch if engineSound exists
        if (engineSound == null) return;

        if (isAccelerating)
        {
            // Increase pitch smoothly
            engineSound.pitch = Mathf.Lerp(engineSound.pitch, maxPitch, Time.deltaTime * 3f);
        }
        else
        {
            // Return back to normal smoothly
            engineSound.pitch = Mathf.Lerp(engineSound.pitch, normalPitch, Time.deltaTime * 3f);
        }
    }

    public void StartAccelerate()
    {
        isAccelerating = true;
    }

    public void StopAccelerate()
    {
        isAccelerating = false;
    }
}
