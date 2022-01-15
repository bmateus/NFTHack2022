using UnityEngine;
using DG.Tweening;

public class UIPanel : MonoBehaviour
{
    RectTransform rectTransform;

    float tweenTime = 0.33f;

    [SerializeField]
    float showOffsetY;

    [SerializeField]
    float hideOffsetY;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    bool IsShowing { get; set; }

    public void Show()
    {
        if (IsShowing)
            return;

        IsShowing = true;

        rectTransform.DOAnchorPosY(showOffsetY, tweenTime);
    }

    public void Hide()
    {
        if (!IsShowing)
            return;

        IsShowing = false;

        rectTransform.DOAnchorPosY(hideOffsetY, tweenTime);
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        if (IsShowing)
            Hide();
        else
            Show();
    }

}
