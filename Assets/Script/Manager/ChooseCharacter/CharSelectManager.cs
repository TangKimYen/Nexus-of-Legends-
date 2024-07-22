using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharSelectManager : MonoBehaviour
{
    private DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SelectCharacter(string characterId, string characterName, string characterAvatarPrefabName)
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

        Debug.Log("Saving Character ID: " + characterId + ", Character Name: " + characterName + ", Avatar Prefab: " + characterAvatarPrefabName + " for user: " + username);

        // Save character information to Firebase
        dbRef.Child("players").Child(username).Child("characterId").SetValueAsync(characterId).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                dbRef.Child("players").Child(username).Child("characterName").SetValueAsync(characterName).ContinueWith(task2 =>
                {
                    if (task2.IsCompleted)
                    {
                        dbRef.Child("players").Child(username).Child("characterAvatarPrefabName").SetValueAsync(characterAvatarPrefabName).ContinueWith(task3 =>
                        {
                            if (task3.IsCompleted)
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                {
                                    Debug.Log("Character selected successfully");
                                    PlayerData.instance.characterId = characterId;
                                    PlayerData.instance.characterName = characterName;
                                    PlayerData.instance.characterAvatarPrefabName = characterAvatarPrefabName;

                                    // Chuy?n sang scene MainLobby sau khi ch?n nh�n v?t
                                    SceneManager.LoadScene("MainLobby");
                                });
                            }
                            else
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                {
                                    Debug.LogError("Failed to save character avatar prefab name: " + task3.Exception);
                                });
                            }
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