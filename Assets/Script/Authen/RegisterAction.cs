using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterAction : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField fullnameInput;
    public TMP_InputField emailInput;
    public TextMeshProUGUI messageText;  // Dùng ?? hi?n th? thông báo, n?u c?n

    /*void Start()
    {
        // Thi?t l?p input type cho password fields
        passwordInput.inputType = TMP_InputField.InputType.Password;
        confirmPasswordInput.inputType = TMP_InputField.InputType.Password;
    }*/

    public void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        string email = emailInput.text;
        string phone = fullnameInput.text;

        if (ValidateRegister(username, password, confirmPassword, email, phone))
        {
            // ??ng ký thành công, chuy?n ??n màn hình chính ho?c màn hình khác
            SceneManager.LoadScene("ChooseCharacter");
        }
        else
        {
            // ??ng ký th?t b?i, hi?n th? thông báo l?i
            if (messageText != null)
            {
                messageText.text = "Invalid registration information.";
                messageText.gameObject.SetActive(true); // Hi?n th? thông báo
            }
        }
    }

    // Hàm gi? ??nh ?? ki?m tra thông tin ??ng ký
    private bool ValidateRegister(string username, string password, string confirmPassword, string email, string fullname)
    {
        // Thay th? ?o?n mã này b?ng logic th?c t? ?? ki?m tra thông tin ??ng ký
        // Ví d?: ki?m tra các ?i?u ki?n nh? ?? dài c?a tên ng??i dùng và m?t kh?u,
        // s? kh?p c?a m?t kh?u và m?t kh?u xác nh?n, ??nh d?ng c?a email, v.v.
        if (string.IsNullOrEmpty(username) ||
               !string.IsNullOrEmpty(password) ||
               password == confirmPassword ||
               !string.IsNullOrEmpty(email) ||
               !string.IsNullOrEmpty(fullname))
        {
            return false;
        }
        // Ki?m tra xem m?t kh?u và m?t kh?u xác nh?n kh?p nhau
        if (password != confirmPassword)
        {
            return false ;
        }
        // Ki?m tra ??nh d?ng c?a email
        string emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        if (!Regex.IsMatch(email, emailPattern))
        {
            return false;
        }

        // Thêm các ki?m tra khác n?u c?n

        return true;
    }
}
