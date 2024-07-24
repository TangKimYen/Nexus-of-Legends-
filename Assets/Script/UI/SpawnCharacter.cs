using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnCharacter : MonoBehaviourPunCallbacks
{
    public GameObject[] characterPrefabs; // Array to hold all player prefabs
    public Transform spawnPoint; // Spawn point for the characters

    // Start is called before the first frame update
    void Start()
    {
        // Get the characterId of the player
        string characterId = PlayerData.instance.characterId;

        // Map the characterId to the index in the prefabs array
        int characterIndex = GetCharacterIndex(characterId);

        if (characterIndex >= 0 && characterIndex < characterPrefabs.Length)
        {
            // Instantiate the character based on the index
            GameObject player = PhotonNetwork.Instantiate(characterPrefabs[characterIndex].name, spawnPoint.position, Quaternion.identity, 0);
        }
        else
        {
            Debug.LogError("Invalid characterId: " + characterId);
        }
    }

    // Function to map characterId to the index in the prefabs array
    private int GetCharacterIndex(string characterId)
    {
        switch (characterId)
        {
            case "c01":
                return 0; // Index of Assassin
            case "c02":
                return 1; // Index of Archer
            case "c03":
                return 2; // Index of Warrior
            case "c04":
                return 3; // Index of Magican
            default:
                return -1; // If characterId is invalid
        }
    }
}
