using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventurerPopup : UIPopup
{
    [SerializeField]
    RawImage portrait;

    [SerializeField]
    TMP_Text name;

    [SerializeField]
    Transform traitRoot;

    [SerializeField]
    GameObject traitPrefab;

    int currentIdx = 0;

    List<Adventurer> adventurers;

    [SerializeField]
    Button nextButton;

    [SerializeField]
    Button previousButton;

    private void Start()
    {
        nextButton.onClick.AddListener(() => SetIndex(currentIdx + 1));
        previousButton.onClick.AddListener(() => SetIndex(currentIdx - 1));
    }

    public void Init(List<Adventurer> adventurers)
    {
        this.adventurers = adventurers;
        SetIndex(currentIdx);
    }

    public void SetIndex(int idx)
    {
        currentIdx = idx;

        var adventurer = adventurers[currentIdx];

        name.text = adventurer.Name;

        foreach( var trait in adventurer.Traits)
        {
            var traitObj = Instantiate(traitPrefab, traitRoot);
            var traitText = traitObj.GetComponentInChildren<TMP_Text>();
            traitText.text = $"{trait.trait_type}: {trait.value}";
        }

        StartCoroutine(adventurer.LoadImage(portrait));

        nextButton.gameObject.SetActive((idx + 1) < (adventurers.Count - 1));
        previousButton.gameObject.SetActive((idx - 1) > 0);


    }

}
