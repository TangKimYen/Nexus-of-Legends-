using Firebase.Database;
using System.Collections;
using UnityEngine;


public class CharSelectManager : MonoBehaviour
{
    private DatabaseReference dbRef;

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
}