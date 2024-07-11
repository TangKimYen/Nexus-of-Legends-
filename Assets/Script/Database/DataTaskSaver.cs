using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;


[Serializable]
public class TaskBaseStats
{
    public string taskName;
    public string taskDescription;
    public string taskRequirement;
    public int taskGold;
    public int taskExp;
    public bool taskStatus;
}
public class DataTaskSaver : MonoBehaviour
{
    public TaskBaseStats taskBaseStats;
    public string taskId;
    DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadTaskData();
    }

    public void SaveTaskData()
    {
        string json = JsonUtility.ToJson(taskBaseStats);
        dbRef.Child("ManageTask").Child(taskId).SetRawJsonValueAsync(json);
    }

    public void LoadTaskData()
    {
        StartCoroutine(LoadTaskDataEnum());
    }

    IEnumerator LoadTaskDataEnum()
    {
        var serverData = dbRef.Child("ManageTask").Child(taskId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Task data is found.");
            taskBaseStats = JsonUtility.FromJson<TaskBaseStats>(jsonData);
        }
        else
        {
            print("Task data is not found.");
        }
    }
}
