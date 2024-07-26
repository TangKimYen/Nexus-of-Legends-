using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;

[Serializable]
public class CharacterBaseStats
{
    public float baseStrength;
    public float baseIntellect;
    public float baseDefense;
    public float baseBlood;
    public float baseMovement;
    public float baseAttackSpeed;
}

public class DataCharacterSaver : MonoBehaviour
{
    public CharacterBaseStats characterBaseStats;
    public string characterId;
    DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadCharacterData();
    }

    public void SaveCharacterData()
    {
        string json = JsonUtility.ToJson(characterBaseStats);
        dbRef.Child("characters").Child(characterId).SetRawJsonValueAsync(json);
    }

    public void LoadCharacterData()
    {
        StartCoroutine(LoadCharacterDataEnum());
    }

    IEnumerator LoadCharacterDataEnum()
    {
        var serverData = dbRef.Child("characters").Child(characterId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Character data is found.");
            characterBaseStats = JsonUtility.FromJson<CharacterBaseStats>(jsonData);
        }
        else
        {
            print("Character data is not found.");
        }
    }
}