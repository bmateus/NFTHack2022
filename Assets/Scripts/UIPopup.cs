using UnityEngine;
using DG.Tweening;

public class UIPopup : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    UIPanel panel;

    [SerializeField]
    GameObject fade;

    float tweenTime = 0.33f;

    private void Start()
    {
        canvasGroup.alpha = 0;

        if ( fade != null)
            fade.SetActive(false);
    }

    public void Show()
    {
        if ( fade != null )
            fade.SetActive(true);

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1.0f, tweenTime);
        panel.Show();
    }


    public void Hide()
    {
        panel.Hide();
        canvasGroup.DOFade(0, tweenTime).OnComplete(() => {
            if (fade != null )
                fade.SetActive(false);
        });
    }


}
