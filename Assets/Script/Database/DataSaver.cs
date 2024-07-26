using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Security.Cryptography;
using System;

[Serializable]
public class PlayerBaseStat
{
    public float baseStrength;
    public float baseIntellect;
    public float baseDefense;
    public float baseBlood;
    public float baseMovement;
    public float baseAttackSpeed;
}

[System.Serializable]
public class PlayerCurrentStats
{
    public float currentStrength;
    public float currentIntellect;
    public float currentDefense;
    public float currentBlood;
    public float currentMovement;
    public float currentAttackSpeed;
}

public class DataSaver : MonoBehaviour
{
    public PlayerBaseStat playerBaseStat;
    public PlayerCurrentStats playerCurrentStat;
    DatabaseReference dbRef;
    // Start is called before the first frame update
    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(playerBaseStat);
        dbRef.Child("PlayerBaseStat").Child(PlayerData.instance.username).SetRawJsonValueAsync(json);
        string jsonCurrent = JsonUtility.ToJson(playerCurrentStat);
        dbRef.Child("PlayerCurrentStat").Child(PlayerData.instance.username).SetRawJsonValueAsync(jsonCurrent);
    }

    public void LoadData() 
    {
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum() 
    {
        var serverData = dbRef.Child("PlayerBaseStat").Child(PlayerData.instance.username).GetValueAsync();
        yield return new WaitUntil(predicate:  () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();   

        if(jsonData != null)
        {
            print("Server data is found.");
            playerBaseStat = JsonUtility.FromJson<PlayerBaseStat>(jsonData);
        }
        else
        {
            print("Server data not found.");
        }
    }
}

