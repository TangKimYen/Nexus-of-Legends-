using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIMainLobby : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text playerLevelText;
    public TMP_Text gemText;
    public TMP_Text goldText;
    public Image avatarImage;
    public Image expBar;
    public TMP_Text expText;

    public Sprite c01AssassinAvatar;
    public Sprite c02ArcherAvatar;
    public Sprite c03WarriorAvatar;
    public Sprite c04MagicanAvatar;

    public int baseExp = 100;
    public float growthFactor = 1.5f;
    public int level;
    public string characterId;
    public float gold;
    public float gem;
    public float currentExp;
    public float expToNextLevel;

    public static PlayerUI Instance { get; private set; }
    private DatabaseReference reference;

    void Awake()
    {
    }

    void Start()
    {
        playerNameText.text = PlayerData.instance.username;
        LoadPlayerData();
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
                        UpdateExpUI(currentExp, expToNextLevel);
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
                                avatarImage.sprite = c02ArcherAvatar; // Hoặc đặt avatar mặc định
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

    public float CalculateExpToNextLevel(int level)
    {
        return Mathf.FloorToInt(baseExp * Mathf.Pow(level, growthFactor));
    }

    public void UpdateExpUI(float currentExp, float expToNextLevel)
    {
        expBar.fillAmount = currentExp / expToNextLevel;
        expText.text = $"{currentExp}/{expToNextLevel}";
    }
}
