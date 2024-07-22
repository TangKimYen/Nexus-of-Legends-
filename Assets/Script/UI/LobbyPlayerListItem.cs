using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerListItem : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text playerLevelText;
    public Image avatarImage;
    public Button inviteButton;

    private string playerId;

    public Sprite c01AssassinAvatar;
    public Sprite c02ArcherAvatar;
    public Sprite c03WarriorAvatar;
    public Sprite c04MagicanAvatar;

    public void SetUp(PlayerLobbyData playerData)
    {
        playerNameText.text = playerData.NickName;
        playerId = playerData.NickName.ToString();
        playerLevelText.text = playerData.Level.ToString(); // Set level text
        inviteButton.onClick.AddListener(() => ConnectToServer.Instance.OnInviteButtonClicked(playerId));
        // Set avatar image based on CharacterId
        switch (playerData.CharacterId)
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
                avatarImage.sprite = null; // Or set a default avatar
                break;
        }
    }
}
