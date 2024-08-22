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
    public bool wasLoggedOutByOtherLogin = false; // Biến cờ để kiểm tra trạng thái đăng xuất
    public bool isLogoutRequested = false; // Thêm cờ để phân biệt khi người dùng nhấn nút Logout
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
        isLogoutRequested = true; // Đánh dấu rằng logout đã được yêu cầu
    }

    public void LogoutAndReturnToTitleScreen()
    {
        Logout();
    }
    public void CheckLogoutReason()
    {
        if (isLogoutRequested)
        {
            // Reset cờ sau khi đã xử lý
            isLogoutRequested = false;
            // Thực hiện các hành động cần thiết sau khi logout yêu cầu
        }
        else if (wasLoggedOutByOtherLogin)
        {
            // Reset cờ sau khi đã xử lý
            wasLoggedOutByOtherLogin = false;
            // Thực hiện các hành động cần thiết sau khi logout do đăng nhập từ nơi khác
        }
    }
}