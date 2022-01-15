using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class DungeonUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text playerHealth;

    [SerializeField]
    TMP_Text numPotions;

    [SerializeField]
    TMP_Text treasuresFound;

    [SerializeField]
    TMP_Text monstersSlain;

    [SerializeField]
    Transform logRoot;

    [SerializeField]
    GameObject logEntryPrefab;

    [SerializeField]
    UIPopup gameOverPopup;

    [SerializeField]
    UIPopup exitDungeonPopup;

    [SerializeField]
    DungeonManager dungeonManager;

    [SerializeField]
    GameObject usePotionButton;

    [SerializeField]
    CanvasGroup damageFlash;

    public void ShowLog(string msg)
    {
        var logEntry = Instantiate(logEntryPrefab, logRoot);
        logEntry.GetComponentInChildren<TMP_Text>().text = msg;
    }

    public void ShowGameOver()
    {
        gameOverPopup.Show();
    }

    public void ShowExitPrompt()
    {
        exitDungeonPopup.Show();
    }

    public void UpdateUI()
    {
        playerHealth.text = $"Health: {dungeonManager.PlayerHealth}/{dungeonManager.MaxPlayerHealth}";
        treasuresFound.text = $"Treasures: {dungeonManager.TreasuresCollected}";
        monstersSlain.text = $"Monsters killed: {dungeonManager.EnemiesKilled}";
        numPotions.text = $"Potions: {dungeonManager.Potions}";
        usePotionButton.SetActive(dungeonManager.Potions > 0);
    }

    private void Update()
    {
        UpdateUI();
    }

    public void ShowDamageFx()
    {
        damageFlash.DOFade(1.0f, 0.1f)
            .OnComplete(() => {
                damageFlash.DOFade(0.0f, 0.1f);
            });
    }

}
