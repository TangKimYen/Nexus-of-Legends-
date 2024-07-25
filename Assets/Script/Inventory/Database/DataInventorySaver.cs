using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string itemId;
    public bool isActive;
}

[Serializable]
public class dataToSaveInventory
{
    public List<ItemData> items;
}

public class DataInventorySaver : MonoBehaviour
{
    public dataToSaveInventory dts;
    public string userName;
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
            dbRef.Child("Inventory").Child(userName).Child(item.itemId).SetRawJsonValueAsync(json);
        }
    }

    public void LoadDataFn()
    {
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum()
    {
        var serverData = dbRef.Child("Inventory").Child(userName).GetValueAsync();
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
                ItemData item = JsonUtility.FromJson<ItemData>(json);
                dts.items.Add(item);
            }
        }
        else
        {
            print("Server no data found");
        }
    }
}
