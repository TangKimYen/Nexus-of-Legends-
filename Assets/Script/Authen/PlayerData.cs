using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;  // Thêm dòng này
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

                    // Xóa tài khoản người dùng khỏi Firebase Auth
                    if (auth.CurrentUser != null)
                    {
                        auth.CurrentUser.DeleteAsync().ContinueWith(deleteTask => {
                            if (deleteTask.IsCompleted)
                            {
                                Debug.Log("Account deleted from Firebase Auth successfully.");
                                
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