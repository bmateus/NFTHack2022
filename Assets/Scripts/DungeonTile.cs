using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class DungeonTile : MonoBehaviour
    ,IPointerEnterHandler
    ,IPointerExitHandler
    ,IPointerClickHandler

{
    [SerializeField] Color baseColor, offsetColor;
    [SerializeField] GameObject highlightObj;

    [SerializeField] Sprite[] dirtSprite;
    [SerializeField] Sprite trapSprite;

    SpriteRenderer sr;

    public Action OnTileSelected;

    public bool IsRevealed { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Init(bool isOffset)
    {
        sr.color = isOffset ? offsetColor : baseColor;
    }


    void OnMouseEnter()
    {
        highlightObj.SetActive(true);
    }

    void OnMouseExit()
    {
        highlightObj.SetActive(false);
    }

    private void OnMouseDown()
    {
        Debug.Log("Click");
        OnTileSelected?.Invoke();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        highlightObj.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        highlightObj.SetActive(false);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        OnTileSelected?.Invoke();
    }

    public void Reveal(bool isTrap = false)
    {
        IsRevealed = true;
        sr.color = Color.white;
        sr.sprite = isTrap? trapSprite : dirtSprite[Random.Range(0, dirtSprite.Length)];
    }


}
