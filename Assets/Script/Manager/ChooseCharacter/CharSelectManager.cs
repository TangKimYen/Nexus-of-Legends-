using Firebase.Database;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharSelectManager : MonoBehaviour
{
    private DatabaseReference dbRef;
    public PlayerBaseStat playerBaseStat;
    public PlayerCurrentStats playerCurrentStats;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SelectCharacter(string characterId, string characterName)
    {
        if (PlayerData.instance == null)
        {
            Debug.LogError("PlayerData.instance is null");
            return;
        }

        if (dbRef == null)
        {
            Debug.LogError("dbRef is null");
            return;
        }

        string username = PlayerData.instance.username;

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Username is null or empty");
            return;
        }

        Debug.Log("Saving Character ID: " + characterId + ", Character Name: " + characterName + " for user: " + username);

        // Save character information to Firebase
        dbRef.Child("players").Child(username).Child("characterId").SetValueAsync(characterId).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                dbRef.Child("players").Child(username).Child("characterName").SetValueAsync(characterName).ContinueWith(task2 =>
                {
                    if (task2.IsCompleted)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            Debug.Log("Character selected successfully");
                            PlayerData.instance.characterId = characterId;
                            PlayerData.instance.characterName = characterName;
                            GetPlayerBaseStat();
                            SavePlayerStat();
                        });
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            Debug.LogError("Failed to save character name: " + task2.Exception);
                        });
                    }
                });
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.LogError("Character selection failed: " + task.Exception);
                });
            }
        });
    }

    public void SavePlayerStat()
    {
        string json = JsonUtility.ToJson(playerBaseStat);
        string jsonCurrentStat = JsonUtility.ToJson(playerCurrentStats);
        dbRef.Child("PlayerBaseStat").Child(PlayerData.instance.username).SetRawJsonValueAsync(json);
        dbRef.Child("PlayerCurrentStat").Child(PlayerData.instance.username).SetRawJsonValueAsync(jsonCurrentStat);
    }

    public void GetPlayerBaseStat()
    {
        StartCoroutine(LoadDataEnum());
    }

    IEnumerator LoadDataEnum()
    {
        var serverData = dbRef.Child("characters").Child(PlayerData.instance.characterId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Server data is found.");
            playerBaseStat = JsonUtility.FromJson<PlayerBaseStat>(jsonData);
            playerCurrentStats = JsonUtility.FromJson<PlayerCurrentStats>(jsonData);
        }
        else
        {
            print("Server data not found.");
        }
    }
}