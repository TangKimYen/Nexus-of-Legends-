using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutAction : MonoBehaviour
{
    public GameObject PopupLog; //Tham chi?u ??n popup login

    // Hàm này s? ???c g?i khi ng??i dùng nh?n nút ??ng xu?t
    public void Logout()
    {
        // Xóa thông tin ng??i dùng ?ã l?u (ví d?: PlayerPrefs)
        PlayerPrefs.DeleteKey("UserToken");

        // Tùy ch?n: Xóa các thông tin khác nh? username, email, v.v.
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Email");
        PlayerPrefs.DeleteKey("Full Name");
        PlayerPrefs.DeleteKey("Password");

        // Chuy?n ??n màn hình ??ng nh?p 
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    }
    private void Start()
    {
        if (PopupLog != null)
        {
            PopupLog.SetActive(true);
        }
    }
}
