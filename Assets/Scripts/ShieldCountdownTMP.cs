using UnityEngine;
using TMPro;

public class ShieldCountdownTMP : MonoBehaviour
{
    public PlayerController player;       // Drag your PlayerController here
    public TextMeshProUGUI shieldText;    // Drag your TMP text here
    public Vector3 offset = new Vector3(0, 2f, 0); // Position above the car

    void Start()
    {
        if (player == null)
            Debug.LogError("ShieldCountdownTMP: PlayerController is not assigned!");
        if (shieldText == null)
            Debug.LogError("ShieldCountdownTMP: TMP Text is not assigned!");

        // Subscribe to the shield countdown event
        player.OnShieldCountdown += UpdateCountdown;
        shieldText.gameObject.SetActive(false); // hide initially
    }

    void LateUpdate()
    {
        if (player != null)
            transform.position = player.transform.position + offset;
    }

    private void UpdateCountdown(float timeLeft)
    {
        if (shieldText == null) return;

        if (timeLeft > 0f)
        {
            shieldText.gameObject.SetActive(true);
            shieldText.text = Mathf.Ceil(timeLeft).ToString(); // rounds up seconds
        }
        else
        {
            shieldText.gameObject.SetActive(false); // hide when shield ends
        }
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnShieldCountdown -= UpdateCountdown;
    }
}
