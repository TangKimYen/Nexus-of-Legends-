using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
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
