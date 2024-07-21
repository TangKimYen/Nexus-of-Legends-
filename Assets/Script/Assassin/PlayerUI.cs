using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    public Text nameText;
    public Image arrowImage;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            nameText.text = photonView.Owner.NickName;
            arrowImage.enabled = true;
        }
        else
        {
            nameText.text = PhotonNetwork.NickName;
            arrowImage.enabled = false;
        }
    }
}
