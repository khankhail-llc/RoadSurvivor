using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    public float pressedScale = 0.9f;
    public float duration = 0.1f;
    public Ease easeType = Ease.OutBack;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(originalScale * pressedScale, duration).SetEase(easeType);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration).SetEase(easeType);
    }

    private void OnDisable()
    {
        // Ensure button resets to original scale if disabled while pressed, to avoid permanent shrinkage
        transform.localScale = originalScale;
    }
}
