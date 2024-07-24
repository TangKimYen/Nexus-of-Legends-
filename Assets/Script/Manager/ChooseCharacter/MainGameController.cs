using UnityEngine;
using UnityEngine.UI;

public class MainLobbyController : MonoBehaviour
{
    public Text usernameText;
    public Image avatarImage;
    public Sprite c01AssassinAvatar;
    public Sprite c02ArcherAvatar;
    public Sprite c03WarriorAvatar;
    public Sprite c04MagicanAvatar;

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
            LoadCharacterAvatar(PlayerData.instance.characterId);
        }
        else
        {
            Debug.LogError("PlayerData instance is null!");
        }
    }

    private void LoadCharacterAvatar(string characterId)
    {
        if (!string.IsNullOrEmpty(characterId))
        {
            switch (characterId)
            {
                case "c01":
                    avatarImage.sprite = c01AssassinAvatar;
                    break;
                case "c02":
                    avatarImage.sprite = c02ArcherAvatar;
                    break;
                case "c03":
                    avatarImage.sprite = c03WarriorAvatar;
                    break;
                case "c04":
                    avatarImage.sprite = c04MagicanAvatar;
                    break;
                default:
                    avatarImage.sprite = c02ArcherAvatar; // Or set a default avatar
                    break;
            }
        }
        else
        {
            Debug.LogError("Character ID is null or empty.");
        }
    }
}