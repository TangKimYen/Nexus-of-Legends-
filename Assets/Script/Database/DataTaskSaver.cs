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
    public TextMeshProUGUI[] TaskNames; // Array of TextMeshProUGUI elements for task names
    public TextMeshProUGUI[] TaskNameContent;
    public TextMeshProUGUI[] TaskDesContent; // Array of TextMeshProUGUI elements for task descriptions
    public TextMeshProUGUI[] TaskRequireContent; // Array of TextMeshProUGUI elements for task requirements
    public TextMeshProUGUI[] TaskGoldContent; // Array of TextMeshProUGUI elements for task gold
    public TextMeshProUGUI[] TaskExpContent; // Array of TextMeshProUGUI elements for task experience points
     public TaskBaseStats taskBaseStats;
    public string taskId; // Add this line to specify which task to load
    DatabaseReference dbRef;
    public Button rewardButton; // Add this line

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        rewardButton.gameObject.SetActive(false); // Hide the reward button initially
        LoadAllTaskData();
    }

    void CheckTaskCompletion()
    {
        if (taskBaseStats.taskStatus)
        {
            rewardButton.gameObject.SetActive(true); // Show the reward button if the task is completed
        }
        else
        {
            rewardButton.gameObject.SetActive(false); // Hide the reward button if the task is not completed
        }
    }

    public void CompleteTask(string taskId)
    {
        dbRef.Child("ManageTasks").Child(taskId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string jsonData = snapshot.GetRawJsonValue();
                if (jsonData != null)
                {
                    taskBaseStats = JsonUtility.FromJson<TaskBaseStats>(jsonData);
                    taskBaseStats.taskStatus = true;
                    SaveTaskData(taskId);
                    CheckTaskCompletion();
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve task data: " + task.Exception);
            }
        });
    }


    public void SaveTaskData(string taskNode)
    {
        string json = JsonUtility.ToJson(taskBaseStats);
        dbRef.Child("ManageTasks").Child(taskId).SetRawJsonValueAsync(json).ContinueWith(task =>
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
        // Check if the arrays are properly assigned and have 8 elements
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
            Debug.Log("Loading data for: " + taskNode); // Debugging

            var serverData = dbRef.Child("ManageTasks").Child(taskNode).GetValueAsync();
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
                Debug.Log("Task data found for " + taskNode);
                taskBaseStats = JsonUtility.FromJson<TaskBaseStats>(jsonData);

                // Display task details in the corresponding TextMeshProUGUI elements
                TaskNames[i - 1].text = taskBaseStats.taskName;
                TaskDesContent[i - 1].text = taskBaseStats.taskDescription;
                TaskRequireContent[i - 1].text = taskBaseStats.taskRequirement;
                TaskGoldContent[i - 1].text = taskBaseStats.taskGold.ToString();
                TaskExpContent[i - 1].text = taskBaseStats.taskExp.ToString();

                CheckTaskCompletion(); // Check if the task is completed
            }
            else
            {
                Debug.LogWarning("Task data not found for " + taskNode);
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
    }




}
