using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text levelText;
    public Image avatarImage;
    public Sprite c01AssassinAvatar;
    public Sprite c02ArcherAvatar;
    public Sprite c03WarriorAvatar;
    public Sprite c04MagicanAvatar;
    [SerializeField] Button kickButton;
    Player player;
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("players").Child(_player.NickName);
        reference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve player data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    int level = int.Parse(snapshot.Child("level").Value.ToString());
                    string characterId = snapshot.Child("characterId").Value.ToString();

                    levelText.text = level.ToString();
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
                    Debug.LogWarning("Player data does not exist in Firebase.");
                }
            }
        });
        kickButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && !player.IsMasterClient);

        kickButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("KickPlayer", player);
            }
        });
    }

    [PunRPC]
    void KickPlayer()
    {
        PhotonNetwork.LeaveRoom();
        ConnectToServer.Instance.kickLayout.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == player)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
