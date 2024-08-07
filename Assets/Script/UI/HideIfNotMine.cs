using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfNotMine : MonoBehaviour
{
    private PhotonView photonView;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.transform.localScale = Vector3.zero;
        }
    }
}
