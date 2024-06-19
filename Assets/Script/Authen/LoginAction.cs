using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginAction : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;  // D�ng ?? hi?n th? th�ng b�o, n?u c?n

    // H�m n�y ???c g?i khi ng??i d�ng nh?n v�o n�t ??ng nh?p
    public void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (ValidateLogin(username, password))
        {
            // ??ng nh?p th�nh c�ng, chuy?n ??n m�n h�nh ch�nh ho?c m�n h�nh kh�c
            SceneManager.LoadScene("NewMainLobby 1");
        }
        else
        {
            // ??ng nh?p th?t b?i, hi?n th? th�ng b�o l?i
            if (messageText != null)
            {
                messageText.text = "Invalid username or password.";
                messageText.gameObject.SetActive(true);
            }
        }
    }

    // H�m gi? ??nh ?? ki?m tra th�ng tin ??ng nh?p
    private bool ValidateLogin(string username, string password)
    {
        // Thay th? ?o?n m� n�y b?ng logic th?c t? ?? ki?m tra th�ng tin ??ng nh?p
        // V� d?: k?t n?i ??n m�y ch? ?? x�c th?c th�ng tin ??ng nh?p
        return username == "player" && password == "1234"; // ?�y ch? l� m?t v� d? ??n gi?n
    }
}
