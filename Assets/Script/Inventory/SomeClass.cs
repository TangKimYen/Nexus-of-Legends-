using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeClass : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Character character;

    private void Start()
    {
        if (PlayerData.instance != null && !string.IsNullOrEmpty(PlayerData.instance.username))
        {
            string username = PlayerData.instance.username;
            inventory.userName = username;
            character.userName = username;

            inventory.LoadItemsFromFirebase();
            character.LoadCharacterData();
        }
        else
        {
            Debug.LogWarning("Unable to set userName for Inventory or Character.");
        }
    }
}
