using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataShopSaver : MonoBehaviour
{
    public dataToSaveInventory dts;
    public string itemId;
    DatabaseReference dbRef;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SaveDataFn()
    {
        foreach (var item in dts.items)
        {
            string json = JsonUtility.ToJson(item);
            dbRef.Child("Shop").Child(itemId).SetRawJsonValueAsync(json);
        }
    }

    public void LoadDataFn()
    {
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum()
    {
        var serverData = dbRef.Child("Shop").Child(itemId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is complete");

        DataSnapshot snapshot = serverData.Result;

        if (snapshot.Exists)
        {
            print("Server data found");

            dts.items.Clear();
            foreach (DataSnapshot itemSnapshot in snapshot.Children)
            {
                string json = itemSnapshot.GetRawJsonValue();
            }
        }
        else
        {
            print("Server no data found");
        }
    }
}
