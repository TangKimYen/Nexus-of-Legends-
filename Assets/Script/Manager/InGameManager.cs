using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameManager : MonoBehaviourPunCallbacks
{
    public static InGameManager instance;
    public int totalEnemies;
    public int defeatedEnemies = 0;
    public int totalGold = 0;
    public int totalXP = 0;

    public GameObject summaryPanel;
    public TMP_Text resultText;
    public TMP_Text killedText;
    public TMP_Text goldText;
    public TMP_Text expText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
        totalEnemies = enemies.Length;
    }

    public void EnemyDefeated(int gold, int xp)
    {
        defeatedEnemies++;
        totalGold += gold;
        totalXP += xp;
        CheckLevelCompletion();
    }

    private void CheckLevelCompletion()
    {
        if (defeatedEnemies == totalEnemies)
        {
            ShowSummary();
        }
    }

    private void ShowSummary()
    {
        summaryPanel.SetActive(true);
        resultText.text = "You Win! Congratulation!";
        goldText.text = $"{totalGold}";
        expText.text = $"{totalXP}";
        killedText.text = $"{defeatedEnemies}";
    }

    public void PlayerDied()
    {
        summaryPanel.SetActive(true);
        resultText.text = "You Died!!!";
        goldText.text = $"{totalGold}";
        expText.text = $"{totalXP}";
        killedText.text = $"{defeatedEnemies}";
    }

    public void ReturnLobby()
    {
        PhotonNetwork.LoadLevel("MainLobby");
    }
}
