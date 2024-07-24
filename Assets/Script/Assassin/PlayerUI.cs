using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    public Text nameText;
    public TMP_Text playerNameText;
    public TMP_Text playerLevelText;
    public TMP_Text gemText;
    public TMP_Text goldText;
    public Image avatarImage;
    public Image arrowImage;
    public Image expBar;
    public TMP_Text expText;

    public Sprite c01AssassinAvatar;
    public Sprite c02ArcherAvatar;
    public Sprite c03WarriorAvatar;
    public Sprite c04MagicanAvatar;

    private PhotonView photonView;
    private int baseExp = 100;
    private float growthFactor = 1.5f;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            nameText.text = PlayerData.instance.username;
            playerNameText.text = PlayerData.instance.username;
            arrowImage.enabled = true;
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("players").Child(PlayerData.instance.username);
            reference.GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve player data from Firebase: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        int level = int.Parse(snapshot.Child("level").Value.ToString());
                        string characterId = snapshot.Child("characterId").Value.ToString();
                        float gold = float.Parse(snapshot.Child("gold").Value.ToString());
                        float gem = float.Parse(snapshot.Child("gem").Value.ToString());
                        float currentExp = float.Parse(snapshot.Child("exp").Value.ToString());

                        float expToNextLevel = CalculateExpToNextLevel(level);

                        playerLevelText.text = "Level: " + level.ToString();
                        goldText.text = gold.ToString();
                        gemText.text = gem.ToString();
                        expBar.fillAmount = currentExp / expToNextLevel;
                        expText.text = $"{currentExp}/{expToNextLevel}";
                        Debug.Log($"Level: {level}, Gold: {gold}, Gem: {gem}, CurrentExp: {currentExp}, ExpToNextLevel: {expToNextLevel}");
                        switch (characterId)
                        {
                            case "c01":
                                avatarImage.sprite = c01AssassinAvatar;
                                break;
                            case "c02":
                                avatarImage.sprite = c02ArcherAvatar;
                                break;
                            case "c03":
                                avatarImage.sprite = c03WarriorAvatar;
                                break;
                            case "c04":
                                avatarImage.sprite = c04MagicanAvatar;
                                break;
                            default:
                                avatarImage.sprite = c02ArcherAvatar; // Or set a default avatar
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Player data does not exist in Firebase.");
                    }
                }
            });
        }
        else
        {
            nameText.text = PhotonNetwork.NickName;
            arrowImage.enabled = false;
        }
    }
    float CalculateExpToNextLevel(int level)
    {
        return Mathf.FloorToInt(baseExp * Mathf.Pow(level, growthFactor));
    }

    public void UpdateExpUI(float currentExp, float expToNextLevel)
    {
        expBar.fillAmount = currentExp / expToNextLevel;
        expText.text = $"{currentExp}/{expToNextLevel}";
    }
}
