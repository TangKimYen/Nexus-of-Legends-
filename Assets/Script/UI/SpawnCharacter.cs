using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnCharacter : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs; // Array to hold all player prefabs

    // Start is called before the first frame update
    void Start()
    {
        // Ensure there are at least 2 prefabs in the array
        if (playerPrefabs.Length < 1)
        {
            Debug.LogError("There must be at least 1 player prefabs assigned in the inspector!");
            return;
        }

        // Randomly select two different indices
        int index1 = Random.Range(0, playerPrefabs.Length);
/*        int index2 = Random.Range(0, playerPrefabs.Length - 1);
        if (index2 >= index1)
            index2++;*/

        // Instantiate the selected player prefabs
        GameObject player1 = PhotonNetwork.Instantiate(playerPrefabs[index1].name, transform.position, Quaternion.identity, 0);
        //GameObject player2 = PhotonNetwork.Instantiate(playerPrefabs[index2].name, new Vector3(Random.Range(-3, 3), 0, 0), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
