using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameController : MonoBehaviour
{
    public GameObject loginRequiredPopup; // Popup yêu cầu đăng nhập
    public TextMeshProUGUI loginRequiredText; // Text UI để hiển thị thông báo

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
                if (loginRequiredText != null)
                {
                    loginRequiredText.text = "You need to login to play the game.";
                }
                loginRequiredPopup.SetActive(true);
            }
        }
    }
}