using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI outOfFuelText;

    private void OnEnable()
    {
        if (FuelManager.Instance != null && FuelManager.Instance.isFuelFinished)
            outOfFuelText.gameObject.SetActive(true);
        else
            outOfFuelText.gameObject.SetActive(false);
    }
}
