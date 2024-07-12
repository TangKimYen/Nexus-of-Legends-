using System.Collections;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using Firebase;

public class LoginAction : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public TextMeshProUGUI usernameDisplayText; // Text để hiển thị username sau khi đăng nhập thành công
    public GameObject loadingScreen;  // Hiển thị khi đang xử lý
    public GameObject loginPopup; // Popup đăng nhập

    private Color successColor;
    private Color errorColor;
    private DatabaseReference dbRef;

    void Start()
    {
        // Chuyển đổi mã màu hex sang Color
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (ValidateLogin(username, password))
        {
            StartCoroutine(LoginUser(username, password));
        }
        else
        {
            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "Invalid login information.";
                messageText.gameObject.SetActive(true); // Hiển thị thông báo
            }
        }
    }

    private bool ValidateLogin(string username, string password)
    {
        // Kiểm tra xem các trường có bị trống không
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        return true;
    }

    private IEnumerator LoginUser(string username, string password)
    {
        loadingScreen.SetActive(true);

        // Lấy email từ username
        var getEmailTask = dbRef.Child("players").Child(username).Child("emailInfo").GetValueAsync();
        yield return new WaitUntil(() => getEmailTask.IsCompleted);

        if (getEmailTask.Exception != null || !getEmailTask.Result.Exists)
        {
            messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
            messageText.text = "Login failed: Invalid username.";
            messageText.gameObject.SetActive(true);
            loadingScreen.SetActive(false);
        }
        else
        {
            string email = getEmailTask.Result.Value.ToString();

            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => loginTask.IsCompleted);

            if (loginTask.Exception != null)
            {
                FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseException.ErrorCode;
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "Login failed: " + errorCode.ToString();
                messageText.gameObject.SetActive(true);
                loadingScreen.SetActive(false);
            }
            else
            {
                AuthResult result = loginTask.Result;
                FirebaseUser user = result.User;
                messageText.color = successColor;  // Thiết lập màu xanh cho thông báo thành công
                messageText.text = "Login successful! Welcome " + user.DisplayName;
                messageText.gameObject.SetActive(true);

                // Hiển thị username sau khi đăng nhập thành công
                if (usernameDisplayText != null)
                {
                    usernameDisplayText.text = "Player: " + username;
                    usernameDisplayText.gameObject.SetActive(true);
                }

                // Lưu tên người dùng
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.Save();

                // Ẩn màn hình loading sau khi đăng nhập thành công
                loadingScreen.SetActive(false);

                // Xóa các trường nhập liệu sau khi đăng nhập thành công
                ResetInputFields();
            }
        }
    }

    private void ResetInputFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
    }

    public void OnCloseLoginPopup()
    {
        // Reset thông báo đăng nhập thành công
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }

        // Ẩn popup đăng nhập khi người dùng nhấn nút close
        if (loginPopup != null)
        {
            loginPopup.SetActive(false);
        }
    }
}