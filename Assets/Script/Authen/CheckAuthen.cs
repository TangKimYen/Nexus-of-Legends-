using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAuthen : MonoBehaviour
{
    public GameObject loggedInUI; // UI hiển thị khi người dùng đã đăng nhập
    public GameObject notLoggedInUI; // UI hiển thị khi người dùng chưa đăng nhập

    void Start()
    {
        // Kiểm tra trạng thái đăng nhập và cập nhật UI tương ứng
        UpdateUIBasedOnLoginStatus();
    }

    private void UpdateUIBasedOnLoginStatus()
    {
        if (PlayerData.instance.isLoggedIn)
        {
            ShowLoggedInUI();
        }
        else
        {
            ShowNotLoggedInUI();
        }
    }

    private void ShowLoggedInUI()
    {
        if (loggedInUI != null)
        {
            loggedInUI.SetActive(true);
        }
        if (notLoggedInUI != null)
        {
            notLoggedInUI.SetActive(false);
        }
    }

    private void ShowNotLoggedInUI()
    {
        if (loggedInUI != null)
        {
            loggedInUI.SetActive(false);
        }
        if (notLoggedInUI != null)
        {
            notLoggedInUI.SetActive(true);
        }
    }
}
