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

        Debug.Log("Saving Character ID: " + characterId + " and Character Name: " + characterName + " for user: " + username);

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
                            PlayerData.instance.characterId = characterId; // Lưu ID của nhân vật vào PlayerData
                            PlayerData.instance.characterName = characterName; // Lưu tên nhân vật vào PlayerData

                            // Chuyển sang scene chính của game sau khi chọn nhân vật
                            SceneManager.LoadScene("MainGameScene");
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