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
    }

    public PlayerData() { }

    public void DeleteAccount()
    {
        if (!string.IsNullOrEmpty(username))
        {
            dbRef.Child("players").Child(username).RemoveValueAsync().ContinueWith(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Account deleted successfully.");
                    Logout();
                    SceneManager.LoadScene("TitleScreen");
                }
                else
                {
                    Debug.LogError("Error deleting account: " + task.Exception);
                }
            });
        }
    }

    public void Logout()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
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
}