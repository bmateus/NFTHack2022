using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField]
    TMP_Text text;

    [SerializeField]
    float onPressOffset = 16f;

    private void OnMouseDown()
    {
        text.transform.localPosition += Vector3.up * onPressOffset;
    }

    private void OnMouseUp()
    {
        text.transform.localPosition -= Vector3.up * onPressOffset;
    }

}
