using UnityEngine;
using UnityEngine.UI;

public class BrakeButton : MonoBehaviour
{
    public PlayerController player;
    public Button brakeButton;

    void Start()
    {
        if (brakeButton == null)
            brakeButton = GetComponent<Button>();

        brakeButton.onClick.AddListener(OnBrakePressed);
    }

    void OnBrakePressed()
    {
        if (player != null)
            player.ApplyBrake();
    }
}