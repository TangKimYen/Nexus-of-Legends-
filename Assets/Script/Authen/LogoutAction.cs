using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutAction : MonoBehaviour
{
    public GameObject PopupLog; //Tham chi?u ??n popup login

    // H�m n�y s? ???c g?i khi ng??i d�ng nh?n n�t ??ng xu?t
    public void Logout()
    {
        // X�a th�ng tin ng??i d�ng ?� l?u (v� d?: PlayerPrefs)
        PlayerPrefs.DeleteKey("UserToken");

        // T�y ch?n: X�a c�c th�ng tin kh�c nh? username, email, v.v.
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Email");
        PlayerPrefs.DeleteKey("Full Name");
        PlayerPrefs.DeleteKey("Password");

        // Chuy?n ??n m�n h�nh ??ng nh?p 
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
