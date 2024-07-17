using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;
using TMPro;

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
    public TextMeshProUGUI[] TaskNames; // Array of TextMeshProUGUI elements for 8 tasks
    public TaskBaseStats taskBaseStats;
    DatabaseReference dbRef;
    public TextMeshProUGUI[] TaskNameContent;
    public TextMeshProUGUI[] TaskDesContent;
    public TextMeshProUGUI[] TaskRequireContent;
    public TextMeshProUGUI[] TaskGoldContent;
    public TextMeshProUGUI[] TaskExpContent;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadAllTaskData();
    }

    public void SaveTaskData(string taskNode)
    {
        string json = JsonUtility.ToJson(taskBaseStats);
        dbRef.Child("ManageTask").Child(taskNode).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Task data saved successfully.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to save task data: " + task.Exception);
            }
        });
    }

    public void LoadAllTaskData()
    {
        StartCoroutine(LoadAllTaskDataEnum());
    }

    IEnumerator LoadAllTaskDataEnum()
    {
        if (TaskNames == null || TaskNames.Length < 8 ||
           TaskDesContent == null || TaskDesContent.Length < 8 ||
           TaskRequireContent == null || TaskRequireContent.Length < 8 ||
           TaskGoldContent == null || TaskGoldContent.Length < 8 ||
           TaskExpContent == null || TaskExpContent.Length < 8)
        {
            Debug.LogError("One or more UI element arrays are not assigned or do not have 8 elements.");
            yield break;
        }

        for (int i = 1; i <= 8; i++)
        {
            string taskNode = "task" + i.ToString("D2");
            var serverData = dbRef.Child("ManageTask").Child(taskNode).GetValueAsync();
            yield return new WaitUntil(predicate: () => serverData.IsCompleted);

            if (serverData.Exception != null)
            {
                Debug.LogError("Failed to retrieve data: " + serverData.Exception);
                yield break;
            }

            DataSnapshot snapshot = serverData.Result;
            string jsonData = snapshot.GetRawJsonValue();

            if (jsonData != null)
            {
                Debug.Log("Task data is found for " + taskNode);
                taskBaseStats = JsonUtility.FromJson<TaskBaseStats>(jsonData);

                // Display task details in the corresponding TextMeshProUGUI elements
                if (TaskNames[i - 1] != null)
                {
                    TaskNames[i - 1].text = taskBaseStats.taskName;
                }
                if (TaskDesContent[i - 1] != null)
                {
                    TaskDesContent[i - 1].text = taskBaseStats.taskDescription;
                }
                if (TaskRequireContent[i - 1] != null)
                {
                    TaskRequireContent[i - 1].text = taskBaseStats.taskRequirement;
                }
                if (TaskGoldContent[i - 1] != null)
                {
                    TaskGoldContent[i - 1].text = taskBaseStats.taskGold.ToString();
                }
                if (TaskExpContent[i - 1] != null)
                {
                    TaskExpContent[i - 1].text = taskBaseStats.taskExp.ToString();
                }
                if (TaskNameContent != null && TaskNames.Length >= i)
                {
                    TaskNameContent[i - 1].text = taskBaseStats.taskName;
                }
                else
                {
                    Debug.LogError("TaskNameContent UI element is not assigned or index out of range.");
                }
            }
            else
            {
                Debug.LogWarning("Task data is not found for " + taskNode);
            }
        }
    }
}


