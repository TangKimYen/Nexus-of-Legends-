using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    public int level;
    public float exp;
    private int expToNextLevel;
    private int baseExp = 100;
    private float growthFactor = 1.5f;

    private PlayerUI playerUI;

    private DatabaseReference databaseReference;

    private void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        LoadPlayerData();
        UpdateExpToNextLevel();
        playerUI.UpdateExpUI(exp, expToNextLevel);
    }

    private void Update()
    {
        SavePlayerData();
    }

    void UpdateExpToNextLevel()
    {
        expToNextLevel = Mathf.FloorToInt(baseExp * Mathf.Pow(level, growthFactor));
    }

    public void AddExp(int amount)
    {
        exp += amount;
        if (exp >= expToNextLevel)
        {
            LevelUp();
        }
        playerUI.UpdateExpUI(exp, expToNextLevel);
    }

    void LevelUp()
    {
        exp -= expToNextLevel;
        level++;
        UpdateExpToNextLevel();
        SavePlayerData();
        playerUI.UpdateExpUI(exp, expToNextLevel);
    }

    void LoadPlayerData()
    {
        // Load data from Firebase
        databaseReference.Child("players").GetValueAsync().ContinueWithOnMainThread(task => 
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                level = int.Parse(snapshot.Child("level").Value.ToString());
                exp = float.Parse(snapshot.Child("exp").Value.ToString());
            }
        });
    }

    void SavePlayerData()
    {
        databaseReference.Child("players").Child(PlayerData.instance.username).Child("level").SetValueAsync(level);
        databaseReference.Child("players").Child(PlayerData.instance.username).Child("exp").SetValueAsync(exp);
    }
}
