using UnityEngine;
using UnityEngine.UI;

public class MainLobbyController : MonoBehaviour
{
    public Text usernameText;
    //public Text characterNameText;
    public Transform avatarParent;

    void Start()
    {
        if (PlayerData.instance != null)
        {
            usernameText.text = "Username: " + PlayerData.instance.username;
            //characterNameText.text = "Character: " + PlayerData.instance.characterName;

            // Hiển thị thêm các thông tin khác nếu cần
            Debug.Log("Session ID: " + PlayerData.instance.sessionId);
            Debug.Log("Login Time: " + PlayerData.instance.loginTime);

            // Tải và hiển thị avatar của nhân vật từ Resources
            LoadCharacterAvatar(PlayerData.instance.characterAvatarPrefabName);
        }
        else
        {
            Debug.LogError("PlayerData instance is null!");
        }
    }

    private void LoadCharacterAvatar(string avatarPrefabName)
    {
        if (!string.IsNullOrEmpty(avatarPrefabName))
        {
            GameObject avatarPrefab = Resources.Load<GameObject>(avatarPrefabName);
            if (avatarPrefab != null)
            {
                GameObject avatarInstance = Instantiate(avatarPrefab, avatarParent);
                Debug.Log("Character avatar loaded successfully: " + avatarPrefabName);
            }
            else
            {
                Debug.LogError("Failed to load character avatar: " + avatarPrefabName);
            }
        }
        else
        {
            Debug.LogError("Character avatar prefab name is null or empty.");
        }
    }
}