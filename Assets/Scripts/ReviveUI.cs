using TMPro;
using UnityEngine;

public class ReviveUI : MonoBehaviour
{
    public static ReviveUI Instance;
    public TMP_Text timerText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateTimer(int time)
    {
        timerText.text = time.ToString();
    }
}
 