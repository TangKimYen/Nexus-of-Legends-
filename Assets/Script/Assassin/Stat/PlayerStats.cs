using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    private PlayerUI playerUI;

    private DatabaseReference databaseReference;

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        if (playerUI == null)
        {
            Debug.LogError("PlayerUI component is not found on this GameObject.");
            return;
        }
    }

    public void AddExp(int amount)
    {
        if (PlayerUI.Instance == null)
        {
            Debug.LogError("PlayerUI.Instance is null. Ensure that PlayerUI is initialized.");
            return;
        }
        PlayerUI.Instance.currentExp += amount;
        if (PlayerUI.Instance.currentExp >= PlayerUI.Instance.expToNextLevel)
        {
            LevelUp();
        }
        PlayerUI.Instance.SavePlayerData();
        playerUI.UpdateExpUI(PlayerUI.Instance.currentExp, PlayerUI.Instance.expToNextLevel);
    }

    void LevelUp()
    {
        if (PlayerUI.Instance == null)
        {
            Debug.LogError("PlayerUI.Instance is null. Ensure that PlayerUI is initialized.");
            return;
        }

        PlayerUI.Instance.currentExp -= PlayerUI.Instance.expToNextLevel;
        PlayerUI.Instance.level++;
        PlayerUI.Instance.CalculateExpToNextLevel(PlayerUI.Instance.level);
        PlayerUI.Instance.SavePlayerData();
        playerUI.UpdateExpUI(PlayerUI.Instance.currentExp, PlayerUI.Instance.expToNextLevel);
    }
}
