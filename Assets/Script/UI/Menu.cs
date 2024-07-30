using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }

    public void PartyButton()
    {
        SceneManager.LoadScene("PartyLobby");
    }
    
    public void ShopButton()
    {
        SceneManager.LoadScene("Shop");
    }
}
