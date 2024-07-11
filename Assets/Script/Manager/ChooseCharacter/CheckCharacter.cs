using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSaveIO : MonoBehaviour
{
    private string selectedCharacter;

    void Start()
    {
        LoadSelectedCharacter();
        if (!string.IsNullOrEmpty(selectedCharacter))
        {
            SpawnCharacter();
        }
        else
        {
            // N?u ng??i ch?i ch?a ch?n nhân v?t, chuy?n h??ng ??n màn hình ch?n nhân v?t
            SceneManager.LoadScene("ChooseCharacter");
        }
    }

    void LoadSelectedCharacter()
    {
        if (PlayerPrefs.HasKey("Select"))
        {
            selectedCharacter = PlayerPrefs.GetString("Select");
        }
    }

    void SpawnCharacter()
    {
        GameObject characterPrefab = Resources.Load<GameObject>("Characters/" + selectedCharacter);
        if (characterPrefab != null)
        {
            Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Character not found: " + selectedCharacter);
        }
    }
}
