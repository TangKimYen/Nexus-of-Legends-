using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnCharacter : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-3, 3), 0, 0), Quaternion.identity, 0);
            if (player.GetComponent<PhotonView>().IsMine)
            {
                Camera.main.GetComponent<CameraController>().SetTarget(player.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
