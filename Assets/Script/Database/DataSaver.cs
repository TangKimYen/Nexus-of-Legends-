using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Security.Cryptography;
using System;

[Serializable]
public class dataToSave
{
    public string userName;
    public int totalGold;
    public int totalGem;
    public int level;
    public float exp;
    public float baseStrength;
    public float baseIntellect;
    public float baseDefense;
    public float baseBlood;
    public float baseMovement;
    public float baseAttackSpeed;
}
public class DataSaver : MonoBehaviour
{
    public dataToSave dts;
    public string userId;
    DatabaseReference dbRef;
    // Start is called before the first frame update
    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(dts);
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void LoadData() 
    {
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum() 
    {
        var serverData = dbRef.Child("users").Child(userId).GetValueAsync();
        yield return new WaitUntil(predicate:  () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();   

        if(jsonData != null)
        {
            print("Server data is found.");
            dts = JsonUtility.FromJson<dataToSave>(jsonData);
        }
        else
        {
            print("Server data not found.");
        }
    }
}

