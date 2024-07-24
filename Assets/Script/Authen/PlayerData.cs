using UnityEngine;

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
}