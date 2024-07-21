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
    public TMP_Text messageText;  // Dùng ?? hi?n th? thông báo, n?u c?n

    // Hàm này ???c g?i khi ng??i dùng nh?n vào nút ??ng nh?p
    public void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (ValidateLogin(username, password))
        {
            // ??ng nh?p thành công, chuy?n ??n màn hình chính ho?c màn hình khác
            SceneManager.LoadScene("NewMainLobby 1");
        }
        else
        {
            // ??ng nh?p th?t b?i, hi?n th? thông báo l?i
            if (messageText != null)
            {
                messageText.text = "Invalid username or password.";
                messageText.gameObject.SetActive(true);
            }
        }
    }

    // Hàm gi? ??nh ?? ki?m tra thông tin ??ng nh?p
    private bool ValidateLogin(string username, string password)
    {
        // Thay th? ?o?n mã này b?ng logic th?c t? ?? ki?m tra thông tin ??ng nh?p
        // Ví d?: k?t n?i ??n máy ch? ?? xác th?c thông tin ??ng nh?p
        return username == "player" && password == "1234"; // ?ây ch? là m?t ví d? ??n gi?n
    }
}
