using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InformationMap
{
    public string title;
    public string description;
    public string dungeon;
    public string levelrequire;
}
public class DataInformationMap : MonoBehaviour
{
    public InformationMap informationMap;
    public string mapId;
    DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadMapData();
    }

    public void SaveMapData()
    {
        string json = JsonUtility.ToJson(informationMap);
        dbRef.Child("map").Child(mapId).SetRawJsonValueAsync(json);
    }

    public void LoadMapData()
    {
        StartCoroutine(LoadMapDataEnum());
    }

    IEnumerator LoadMapDataEnum()
    {
        var serverData = dbRef.Child("map").Child(mapId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Map data is found.");
            informationMap = JsonUtility.FromJson<InformationMap>(jsonData);
        }
        else
        {
            print("Map data is not found.");
        }
    }
}
