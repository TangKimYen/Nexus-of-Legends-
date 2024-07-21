using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharSelectManager : MonoBehaviour
{
    public void SelectCharacter(string characterName)
    {
        PlayerPrefs.SetString("Select", characterName);
        PlayerPrefs.Save();
        LoadGame();
    }

    public void LoadGame()
    {
        // Gi? s? "GameScene" l� t�n c?a scene ch�nh c?a b?n
        SceneManager.LoadScene("GameScene");
    }
}
