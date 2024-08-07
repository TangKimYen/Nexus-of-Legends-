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
    public Image skill1;
    public Image skill2;
    public Image skill3;
    public Image expBar;
    public TMP_Text expText;
    public int health;
    public TMP_Text healthText;

    public Sprite c01AssassinAvatar;
    public Sprite c02ArcherAvatar;
    public Sprite c03WarriorAvatar;
    public Sprite c04MagicanAvatar;
    public Sprite[] skillAssassin;
    public Sprite[] skillArcher;
    public Sprite[] skillWarrior;
    public Sprite[] skillMagican;

    public int baseExp = 100;
    public float growthFactor = 1.5f;
    public int level;
    public string characterId;
    public float gold;
    public float gem;
    public float currentExp;
    public float expToNextLevel;

    public static PlayerUI Instance { get; private set; }
    public static PlayerUIMainLobby MainLobbyUIInstance;
    private DatabaseReference reference;
    private DatabaseReference referenceHealth;

    void Start()
    {
        playerNameText.text = PlayerData.instance.username;
        LoadPlayerData();
    }

    private void Awake()
    {
        MainLobbyUIInstance = this;
    }

    public void LoadPlayerData()
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
                                skill1.sprite = skillAssassin[0];
                                skill2.sprite = skillAssassin[1];
                                skill3.sprite = skillAssassin[2];
                                break;
                            case "c02":
                                avatarImage.sprite = c02ArcherAvatar;
                                skill1.sprite = skillArcher[0];
                                skill2.sprite = skillArcher[1];
                                skill3.sprite = skillArcher[2];
                                break;
                            case "c03":
                                avatarImage.sprite = c03WarriorAvatar;
                                skill1.sprite = skillWarrior[0];
                                skill2.sprite = skillWarrior[1];
                                skill3.sprite = skillWarrior[2];
                                break;
                            case "c04":
                                avatarImage.sprite = c04MagicanAvatar;
                                skill1.sprite = skillMagican[0];
                                skill2.sprite = skillMagican[1];
                                skill3.sprite = skillMagican[2];
                                break;
                            default:
                                avatarImage.sprite = c02ArcherAvatar; // Hoặc đặt avatar mặc định
                                skill1.sprite = skillArcher[0];
                                skill2.sprite = skillArcher[1];
                                skill3.sprite = skillArcher[2];
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
        referenceHealth = FirebaseDatabase.DefaultInstance.GetReference("PlayerCurrentStat").Child(PlayerData.instance.username);
        referenceHealth.GetValueAsync().ContinueWithOnMainThread(taskk => {
            if (taskk.IsFaulted)
            {
                Debug.LogError("Unable to retrieve player data from Firebase: " + taskk.Exception);
            }
            else if (taskk.IsCompleted)
            {
                DataSnapshot snapshott = taskk.Result;
                if (snapshott.Exists)
                {
                    try
                    {
                        health = int.Parse(snapshott.Child("currentBlood").Value.ToString());
                        healthText.text = $"{health}/{health}";
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
