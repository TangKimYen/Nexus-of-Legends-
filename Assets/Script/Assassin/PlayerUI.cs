using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    public TMP_Text nameText;
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
    public int baseExp = 100;
    public float growthFactor = 1.5f;
    public int level;
    public string characterId;
    public float gold;
    public float gem;
    public float currentExp;
    public float expToNextLevel;

    public Canvas AvatarCanvas;

    public static PlayerUI Instance { get; private set; }
    private DatabaseReference reference;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            nameText.text = PlayerData.instance.username;
            playerNameText.text = PlayerData.instance.username;
            arrowImage.enabled = true;
            LoadPlayerData();
        }
        else
        {
            arrowImage.enabled = false;
        }
    }

    void LoadPlayerData()
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference("players").Child(PlayerData.instance.username);
        reference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Unable to retrieve player data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    try
                    {
                        level = int.Parse(snapshot.Child("level").Value.ToString());
                        characterId = snapshot.Child("characterId").Value.ToString();
                        gold = float.Parse(snapshot.Child("gold").Value.ToString());
                        gem = float.Parse(snapshot.Child("gem").Value.ToString());
                        currentExp = float.Parse(snapshot.Child("exp").Value.ToString());
                        expToNextLevel = CalculateExpToNextLevel(level);

                        Debug.Log($"Level: {level}, Gold: {gold}, Gem: {gem}, CurrentExp: {currentExp}, ExpToNextLevel: {expToNextLevel}");

                        playerLevelText.text = "Level: " + level.ToString();
                        goldText.text = gold.ToString();
                        gemText.text = gem.ToString();
                        photonView.RPC("UpdatePlayerInfoRPC", RpcTarget.AllBuffered, PlayerData.instance.username, level, gold, gem, currentExp, expToNextLevel, characterId);
                        UpdateExpUI(currentExp, expToNextLevel, level);
                        UpdateGoldUI(gold);
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
                                avatarImage.sprite = c02ArcherAvatar;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error processing player data: " + ex.Message);
                    }
                }
                else
                {
                    Debug.LogWarning("Player data does not exist in Firebase.");
                }
            }
        });
    }

    [PunRPC]
    private void UpdatePlayerInfoRPC(string name, int newLevel, float newGold, float newGem, float newCurrentExp, float newExpToNextLevel, string newCharacterId)
    {
        nameText.text = name;
        playerLevelText.text = "Level: " + newLevel.ToString();
        goldText.text = newGold.ToString();
        gemText.text = newGem.ToString();
        expBar.fillAmount = newCurrentExp / newExpToNextLevel;
        expText.text = $"{newCurrentExp}/{newExpToNextLevel}";
        characterId = newCharacterId;
        UpdateAvatar();
    }

    public float CalculateExpToNextLevel(int level)
    {
        return Mathf.FloorToInt(baseExp * Mathf.Pow(level, growthFactor));
    }

    public void UpdateExpUI(float currentExp, float expToNextLevel, int level)
    {
        expBar.fillAmount = currentExp / expToNextLevel;
        expText.text = $"{currentExp}/{expToNextLevel}";
        playerLevelText.text = "Level: " + level.ToString();
    }

    public void UpdateGoldUI(float gold)
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
        }
    }

    public void SavePlayerData()
    {
        reference.Child("level").SetValueAsync(level);
        reference.Child("exp").SetValueAsync(currentExp);
        reference.Child("gold").SetValueAsync(gold);
    }

    public void UpdateAvatar()
    {
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
                avatarImage.sprite = c02ArcherAvatar;
                break;
        }
    }
}
