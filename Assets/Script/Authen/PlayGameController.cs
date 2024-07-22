using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameController : MonoBehaviour
{
    public GameObject loginRequiredPopup; // Popup yêu cầu đăng nhập

    public void OnPlayGameButtonClicked()
    {
        if (PlayerData.instance != null && PlayerData.instance.isLoggedIn)
        {
            SceneManager.LoadScene("MainLobby");
        }
        else
        {
            Debug.Log("Player is not logged in. Show login required popup.");
            // Hiển thị popup yêu cầu đăng nhập nếu cần
            if (loginRequiredPopup != null)
            {
                loginRequiredPopup.SetActive(true);
            }
        }
    }
}