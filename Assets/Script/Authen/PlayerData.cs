using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    public string playerId;
    public string username;
    public string email;
    public string passwordHash;
    public string characterId = "";
    public string characterName = ""; // Thêm thuộc tính này
    public float exp;
    public float gold;
    public float gem;
    public string sessionId;
    public string loginTime;
    public string logoutTime;
    public int level;

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

    public PlayerData(string NickName, int Level, string CharacterId)
    {
        username = NickName;
        level = Level;
        characterId = CharacterId;
    }
}