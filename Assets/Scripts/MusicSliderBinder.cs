using UnityEngine;
using UnityEngine.UI;

public class MusicSliderBinder : MonoBehaviour
{
    private void OnEnable()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.RegisterSlider(GetComponent<Slider>());
        }
    }
}
