using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    public string playerId;
    public string username;
    public string email;
    public string passwordHash;
    public string characterId = "";
    public string characterName = "";
    public float exp;
    public float gold;
    public float gem;
    public int level; // Thêm thuộc tính level
    public string sessionId;
    public string loginTime;
    public string logoutTime;
    public bool isLoggedIn;
    private DatabaseReference dbRef;
    private FirebaseAuth auth;
    // Thêm các thuộc tính để lưu trạng thái âm thanh
    public bool isMusicOn = true; // Mặc định là bật
    public bool isCharacterSoundOn = true; // Mặc định là bật
    public AudioSource[] characterAudioSources; // Thêm thuộc tính này


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerData instance created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate PlayerData instance destroyed.");
        }
    }

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        // Tải trạng thái âm thanh khi bắt đầu
        LoadSoundSettings();
        characterAudioSources = FindObjectsOfType<AudioSource>();
    }

    public PlayerData() { }


    public void DeleteAccount()
    {
        if (!string.IsNullOrEmpty(username))
        {
            // Xóa thông tin người dùng trong Realtime Database
            dbRef.Child("players").Child(username).RemoveValueAsync().ContinueWith(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Account deleted from Realtime Database successfully.");

                    // Xóa thông tin từ playerBaseStat
                    dbRef.Child("PlayerBaseStat").Child(username).RemoveValueAsync().ContinueWith(baseStatTask => {
                        if (baseStatTask.IsCompleted)
                        {
                            Debug.Log("PlayerBaseStat deleted successfully.");
                        }
                        else
                        {
                            Debug.LogError("Error deleting PlayerBaseStat: " + baseStatTask.Exception);
                        }
                    });

                    // Xóa thông tin từ playerCurrentStat
                    dbRef.Child("PlayerCurrentStat").Child(username).RemoveValueAsync().ContinueWith(currentStatTask => {
                        if (currentStatTask.IsCompleted)
                        {
                            Debug.Log("PlayerCurrentStat deleted successfully.");
                        }
                        else
                        {
                            Debug.LogError("Error deleting PlayerCurrentStat: " + currentStatTask.Exception);
                        }
                    });

                    // Xóa thông tin từ Inventory
                    dbRef.Child("Inventory").Child(username).RemoveValueAsync().ContinueWith(inventoryTask => {
                        if (inventoryTask.IsCompleted)
                        {
                            Debug.Log("Inventory deleted successfully.");
                        }
                        else
                        {
                            Debug.LogError("Error deleting Inventory: " + inventoryTask.Exception);
                        }
                    });

                    // Xóa tài khoản người dùng khỏi Firebase Auth
                    if (auth.CurrentUser != null)
                    {
                        auth.CurrentUser.DeleteAsync().ContinueWith(deleteTask => {
                            if (deleteTask.IsCompleted)
                            {
                                Debug.Log("Account deleted from Firebase Auth successfully.");
                                // Sau khi xóa, chuyển đến màn hình đăng nhập
                                SceneManager.LoadScene("TitleScreen");
                            }
                            else
                            {
                                Debug.LogError("Error deleting account from Firebase Auth: " + deleteTask.Exception);
                            }
                        });
                    }
                }
                else
                {
                    Debug.LogError("Error deleting account from Realtime Database: " + task.Exception);
                }
            });
        }
    }
    // Phương thức lưu trạng thái âm thanh vào Firebase
    public void SaveSoundSettings()
    {
        if (!string.IsNullOrEmpty(username))
        {
            dbRef.Child("players").Child(username).Child("SoundSettings").Child("MusicOn").SetValueAsync(isMusicOn);
            dbRef.Child("players").Child(username).Child("SoundSettings").Child("CharacterSoundOn").SetValueAsync(isCharacterSoundOn);
        }
    }

    // Phương thức tải trạng thái âm thanh từ Firebase
    public void LoadSoundSettings()
    {
        if (!string.IsNullOrEmpty(username))
        {
            dbRef.Child("players").Child(username).Child("SoundSettings").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    DataSnapshot snapshot = task.Result;
                    isMusicOn = snapshot.Child("MusicOn").Value != null && bool.Parse(snapshot.Child("MusicOn").Value.ToString());
                    isCharacterSoundOn = snapshot.Child("CharacterSoundOn").Value != null && bool.Parse(snapshot.Child("CharacterSoundOn").Value.ToString());
                }
            });
        }
    }


    public void Logout()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }

        playerId = "";
        username = "";
        email = "";
        passwordHash = "";
        characterId = "";
        characterName = "";
        exp = 0;
        gold = 0;
        gem = 0;
        level = 0;
        sessionId = "";
        loginTime = "";
        logoutTime = "";
        isLoggedIn = false;
    }

    public void LogoutAndReturnToTitleScreen()
    {
        Logout();
    }
}