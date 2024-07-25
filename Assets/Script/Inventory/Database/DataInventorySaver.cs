using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;

[Serializable]
public class ItemData
{
    public string itemId;
    public bool isActive;  // Add this field
}

[Serializable]
public class dataToSaveInventory
{
    public List<ItemData> items;  // Thay đổi từ một mục đơn lẻ sang danh sách các mục
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
        string json = JsonUtility.ToJson(dts);
        dbRef.Child("Inventory").Child(userName).SetRawJsonValueAsync(json);
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
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Server data found");
            dts = JsonUtility.FromJson<dataToSaveInventory>(jsonData);
        }
        else
        {
            print("Server no data found");
        }
    }

}