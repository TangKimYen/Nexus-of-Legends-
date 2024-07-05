using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;

[Serializable]
public class MonsterStats
{
    public string monsterName;
    public float health;
    public float strength;
    public float intellect;
    public float defense;
}

public class DataEnemiesSaver : MonoBehaviour
{
    public MonsterStats monsterStats;
    public string monsterId;
    DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadMonsterData();
    }

    public void SaveMonsterData()
    {
        string json = JsonUtility.ToJson(monsterStats);
        dbRef.Child("monsters").Child(monsterId).SetRawJsonValueAsync(json);
    }

    public void LoadMonsterData()
    {
        StartCoroutine(LoadMonsterDataEnum());
    }

    IEnumerator LoadMonsterDataEnum()
    {
        var serverData = dbRef.Child("monsters").Child(monsterId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Monster data is found.");
            monsterStats = JsonUtility.FromJson<MonsterStats>(jsonData);
        }
        else
        {
            print("Monster data is not found.");
        }
    }

    public float CalculateMonsterDamage()
    {
        float baseDamage = monsterStats.strength * 1.1f + monsterStats.intellect * 1.5f;
        return baseDamage;
    }
}