using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportPortal : MonoBehaviourPunCallbacks
{
    public GameObject destination;
    GameObject player;

    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Character");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Character"))
        {
            if (Vector2.Distance(player.transform.position, transform.position) > 0.1f)
            {
                player.transform.position = destination.transform.position;
            }
        }
    }
}
