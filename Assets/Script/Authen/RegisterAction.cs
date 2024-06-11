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
    public TextMeshProUGUI messageText;  // D�ng ?? hi?n th? th�ng b�o, n?u c?n

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
            // ??ng k� th�nh c�ng, chuy?n ??n m�n h�nh ch�nh ho?c m�n h�nh kh�c
            SceneManager.LoadScene("ChooseCharacter");
        }
        else
        {
            // ??ng k� th?t b?i, hi?n th? th�ng b�o l?i
            if (messageText != null)
            {
                messageText.text = "Invalid registration information.";
                messageText.gameObject.SetActive(true); // Hi?n th? th�ng b�o
            }
        }
    }

    // H�m gi? ??nh ?? ki?m tra th�ng tin ??ng k�
    private bool ValidateRegister(string username, string password, string confirmPassword, string email, string fullname)
    {
        // Thay th? ?o?n m� n�y b?ng logic th?c t? ?? ki?m tra th�ng tin ??ng k�
        // V� d?: ki?m tra c�c ?i?u ki?n nh? ?? d�i c?a t�n ng??i d�ng v� m?t kh?u,
        // s? kh?p c?a m?t kh?u v� m?t kh?u x�c nh?n, ??nh d?ng c?a email, v.v.
        if (string.IsNullOrEmpty(username) ||
               !string.IsNullOrEmpty(password) ||
               password == confirmPassword ||
               !string.IsNullOrEmpty(email) ||
               !string.IsNullOrEmpty(fullname))
        {
            return false;
        }
        // Ki?m tra xem m?t kh?u v� m?t kh?u x�c nh?n kh?p nhau
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

        // Th�m c�c ki?m tra kh�c n?u c?n

        return true;
    }
}
