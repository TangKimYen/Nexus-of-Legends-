using System.Collections;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase;
using System.Text.RegularExpressions;

public class PasswordResetAction : MonoBehaviour
{
    public TMP_InputField oldPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public GameObject loadingScreen;  // Hiển thị khi đang xử lý
    public GameObject resetPasswordPopup; // Popup đặt lại mật khẩu

    private Color successColor;
    private Color errorColor;

    void Start()
    {
        // Chuyển đổi mã màu hex sang Color 
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnResetPasswordButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }
    }

    private void SelectNextInputField()
    {
        if (oldPasswordInput.isFocused)
        {
            newPasswordInput.Select();
        }
        else if (newPasswordInput.isFocused)
        {
            confirmPasswordInput.Select();
        }
        else if (confirmPasswordInput.isFocused)
        {
            oldPasswordInput.Select();
        }
    }

    public void OnResetPasswordButtonClicked()
    {
        string oldPassword = oldPasswordInput.text;
        string newPassword = newPasswordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        string validationMessage = ValidatePasswords(oldPassword, newPassword, confirmPassword);
        if (validationMessage == null)
        {
            StartCoroutine(ResetPassword(oldPassword, newPassword));
        }
        else
        {
            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = validationMessage;
                messageText.gameObject.SetActive(true); // Hiển thị thông báo
            }
        }
    }

    private string ValidatePasswords(string oldPassword, string newPassword, string confirmPassword)
    {
        // Kiểm tra xem các trường có bị trống không
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            return "Fields cannot be empty.";
        }
        // Kiểm tra xem mật khẩu mới có trùng với mật khẩu cũ không
        if (newPassword == oldPassword)
        {
            return "New password cannot be the same as the old password.";
        }
        // Kiểm tra định dạng của mật khẩu mới: chỉ chứa số và có độ dài tối thiểu 6 ký tự
        string passwordPattern = @"^\d{6,}$";
        if (!Regex.IsMatch(newPassword, passwordPattern))
        {
            return "New password must be at least 6 digits long.";
        }

        // Kiểm tra xem mật khẩu mới và xác nhận mật khẩu có khớp không
        if (newPassword != confirmPassword)
        {
            return "Password confirmation does not match.";
        }

        return null;
    }

    private IEnumerator ResetPassword(string oldPassword, string newPassword)
    {
        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        // Reauthenticate the user
        Credential credential = EmailAuthProvider.GetCredential(user.Email, oldPassword);
        var reauthTask = user.ReauthenticateAsync(credential);
        yield return new WaitUntil(() => reauthTask.IsCompleted);

        if (reauthTask.Exception != null)
        {
            FirebaseException firebaseException = reauthTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseException.ErrorCode;

            // Kiểm tra lỗi và hiển thị thông báo phù hợp
            if (errorCode == AuthError.WrongPassword)
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "The old password is incorrect. Please try again.";
            }
            else
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "The current password is incorrect.";// + errorCode.ToString();
            }

            messageText.gameObject.SetActive(true);
            loadingScreen.SetActive(false);
        }
        else
        {
            // Update the password
            var updatePasswordTask = user.UpdatePasswordAsync(newPassword);
            yield return new WaitUntil(() => updatePasswordTask.IsCompleted);

            if (updatePasswordTask.Exception != null)
            {
                FirebaseException firebaseException = updatePasswordTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "Password reset failed: " + errorCode.ToString();
                messageText.gameObject.SetActive(true);
                loadingScreen.SetActive(false);
            }
            else
            {
                messageText.color = successColor;  // Thiết lập màu xanh cho thông báo thành công
                messageText.text = "Password reset successful!";
                messageText.gameObject.SetActive(true);

                // Cập nhật mật khẩu mới vào PlayerData (nếu cần)
                PlayerData.instance.passwordHash = newPassword;

                // Đăng xuất người dùng và chuyển về màn hình tiêu đề
                PlayerData.instance.Logout();
                SceneManager.LoadScene("TitleScreen"); // Thay "TitleScreen" bằng tên scene màn hình tiêu đề của bạn

                loadingScreen.SetActive(false);
            }
        }
    }

    public void OnCloseResetPasswordPopup()
    {
        // Reset thông báo đặt lại mật khẩu
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }

        // Reset các trường nhập liệu
        ResetInputFields();

        // Ẩn popup đặt lại mật khẩu khi người dùng nhấn nút close
        if (resetPasswordPopup != null)
        {
            resetPasswordPopup.SetActive(false);
        }
    }

    private void ResetInputFields()
    {
        oldPasswordInput.text = "";
        newPasswordInput.text = "";
        confirmPasswordInput.text = "";
    }
}