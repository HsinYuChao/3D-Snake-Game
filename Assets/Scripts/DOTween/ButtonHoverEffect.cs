using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public float duration = 0.2f;

    private Vector3 originalScale;
    private Tween currentTween;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill(); // 停止舊動畫
        currentTween = transform.DOScale(originalScale * hoverScale, duration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentTween?.Kill(); // 停止舊動畫
        currentTween = transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }
}

