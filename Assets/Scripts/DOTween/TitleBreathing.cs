using UnityEngine;
using DG.Tweening; // 引用 DOTween

public class TitleBreathing : MonoBehaviour
{
    public float scaleAmount = 1.1f;  // 呼吸放大倍率
    public float duration = 1.2f;     // 呼吸週期時間

    void Start()
    {
        if (gameObject.activeInHierarchy)
        {
            transform.DOScale(Vector3.one * scaleAmount, duration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
        }
    }

}

