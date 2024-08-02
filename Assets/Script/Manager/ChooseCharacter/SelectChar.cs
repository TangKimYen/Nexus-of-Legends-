using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectChar : MonoBehaviour
{
    public GameObject confirmationPopup;
    public TextMeshProUGUI confirmationText;

    private string characterId; // ID của nhân vật
    private string characterName; // Tên của nhân vật
    public CharSelectManager charSelectManager; // Gán trực tiếp qua Inspector
    private DatabaseReference dbRef;
    public PlayerBaseStat playerBaseStat;
    public PlayerCurrentStats playerCurrentStats;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        if (charSelectManager == null)
        {
            Debug.LogError("CharSelectManager not assigned.");
        }
    }

    public void SelectFireKnight()
    {
        characterId = "c03";
        characterName = "Fire Knight";
        ShowConfirmationPopup();
    }

    public void SelectLeafRanger()
    {
        characterId = "c02";
        characterName = "Leaf Ranger";
        ShowConfirmationPopup();
    }

    public void SelectWaterPriestess()
    {
        characterId = "c04";
        characterName = "Water Priestess";
        ShowConfirmationPopup();
    }

    public void SelectWindAssassin()
    {
        characterId = "c01";
        characterName = "Wind Assassin";
        ShowConfirmationPopup();
    }

    private void ShowConfirmationPopup()
    {
        if (confirmationText != null)
        {
            confirmationText.text = "Are you sure you want to select " + characterName + " as your character?";
            if (confirmationPopup != null)
            {
                confirmationPopup.SetActive(true);
                Debug.Log("Showing confirmation popup for " + characterName);
            }
            else
            {
                Debug.LogError("Confirmation popup is not assigned.");
            }
        }
        else
        {
            Debug.LogError("Confirmation text is not assigned.");
        }
    }

    public void ConfirmSelection()
    {
        Debug.Log("Confirmed selection of Character ID: " + characterId);
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(false);
        }
        else
        {
            Debug.LogError("Confirmation popup is not assigned.");
        }

        if (charSelectManager != null)
        {
            string username = PlayerData.instance?.username;
            if (string.IsNullOrEmpty(username))
            {
                Debug.LogError("Username is null or empty");
                return;
            }

            // Gọi hàm SelectCharacter của CharSelectManager với 2 đối số
            charSelectManager.SelectCharacter(characterId, characterName);
            CoroutineRunner.Instance.StartCoroutine(LoadAndSaveCharacterStats(username, characterId));
            SceneManager.LoadScene("TitleScreen");
        }
        else
        {
            Debug.LogError("charSelectManager is null.");
        }
    }

    public void CancelSelection()
    {
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(false);
        }
        else
        {
            Debug.LogError("Confirmation popup is not assigned.");
        }
    }

    private IEnumerator LoadAndSaveCharacterStats(string username, string characterId)
    {
        // Load character stats from Firebase
        yield return CoroutineRunner.Instance.StartCoroutine(LoadDataEnum(characterId));

        if (playerBaseStat != null)
        {
            // Save character base stats to Firebase under the player's node
            string jsonBaseStat = JsonUtility.ToJson(playerBaseStat);
            dbRef.Child("PlayerBaseStat").Child(username).SetRawJsonValueAsync(jsonBaseStat)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Lưu chỉ số cơ bản thành công cho " + username);
                    }
                    else
                    {
                        Debug.LogError("Lưu chỉ số cơ bản thất bại: " + task.Exception);
                    }
                });

            // Initialize playerCurrentStats with playerBaseStat
            playerCurrentStats = new PlayerCurrentStats
            {
                currentAttackSpeed = playerBaseStat.baseAttackSpeed,
                currentBlood = playerBaseStat.baseBlood,
                currentDefense = playerBaseStat.baseDefense,
                currentIntellect = playerBaseStat.baseIntellect,
                currentMovement = playerBaseStat.baseMovement,
                currentStrength = playerBaseStat.baseStrength
            };

            string jsonCurrentStats = JsonUtility.ToJson(playerCurrentStats);
            dbRef.Child("PlayerCurrentStat").Child(username).SetRawJsonValueAsync(jsonCurrentStats)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Lưu chỉ số hiện tại thành công cho " + username);
                    }
                    else
                    {
                        Debug.LogError("Lưu chỉ số hiện tại thất bại: " + task.Exception);
                    }
                });
        }
    }

    private IEnumerator LoadDataEnum(string characterId)
    {
        var serverData = dbRef.Child("characters").Child(characterId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        Debug.Log("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            Debug.Log("Server data is found.");
            playerBaseStat = JsonUtility.FromJson<PlayerBaseStat>(jsonData);
        }
        else
        {
            Debug.Log("Server data not found.");
        }
    }
}