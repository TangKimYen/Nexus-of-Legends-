using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
<<<<<<< HEAD
using Firebase.Auth;
using Firebase.Extensions;
using Firebase;
using Firebase.Database;
using UnityEngine.SceneManagement;

public class PlayerInfo
{
    public string usernameInfo;
    public string emailInfo;
    public string passwordHash;
    public string characterId;
    public float exp;
    public float gold;
    public float gem;

    public PlayerInfo(string usernameInfo, string emailInfo, string passwordHash, string characterId, float exp, float gold, float gem)
    {
        this.usernameInfo = usernameInfo;
        this.emailInfo = emailInfo;
        this.passwordHash = passwordHash;
        this.characterId = characterId;
        this.exp = exp;
        this.gold = gold;
        this.gem = gem;
    }
}
=======
using UnityEngine.SceneManagement;
>>>>>>> main

public class RegisterAction : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField fullnameInput;
    public TMP_InputField emailInput;
<<<<<<< HEAD
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public TextMeshProUGUI usernameDisplayText; // Text để hiển thị username sau khi đăng ký thành công
    public GameObject loadingScreen;  // Hiển thị khi đang xử lý
    public GameObject registerPopup; // Popup đăng ký

    // Định nghĩa các màu tùy chỉnh bằng mã màu hex
    private Color successColor;
    private Color errorColor;

    DatabaseReference dbRef;

    void Start()
=======
    public TextMeshProUGUI messageText;  // D�ng ?? hi?n th? th�ng b�o, n?u c?n

    /*void Start()
>>>>>>> main
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
<<<<<<< HEAD

    private IEnumerator CheckUsernameAndRegister(string username, string email, string password)
    {
        loadingScreen.SetActive(true);

        var usernameCheckTask = dbRef.Child("players").Child(username).GetValueAsync();
        yield return new WaitUntil(() => usernameCheckTask.IsCompleted);

        if (usernameCheckTask.Exception != null)
        {
            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "Error checking username.";
                messageText.gameObject.SetActive(true); // Hiển thị thông báo
            }
            loadingScreen.SetActive(false);
        }
        else if (usernameCheckTask.Result.Exists)
        {
            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "Username is already taken.";
                messageText.gameObject.SetActive(true); // Hiển thị thông báo
            }
            loadingScreen.SetActive(false);
        }
        else
        {
            StartCoroutine(RegisterUser(username, email, password));
        }
    }

    private IEnumerator RegisterUser(string username, string email, string password)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseException.ErrorCode;
            string errorMessage;

            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    errorMessage = "Email is already in use.";
                    break;
                case AuthError.InvalidEmail:
                    errorMessage = "Invalid email address.";
                    break;
                case AuthError.WeakPassword:
                    errorMessage = "Password is too weak.";
                    break;
                default:
                    errorMessage = "Registration failed: " + errorCode.ToString();
                    break;
            }

            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = errorMessage;
                messageText.gameObject.SetActive(true);
            }
            loadingScreen.SetActive(false);
        }
        else
        {
            AuthResult result = registerTask.Result;
            FirebaseUser user = result.User;
            UserProfile profile = new UserProfile { DisplayName = username };
            var profileTask = user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(() => profileTask.IsCompleted);

            string characterId = ""; // Để trống ban đầu
            float exp = 0;
            float gold = 0;
            float gem = 0;
            string passwordHash = ComputeHash(password); // Băm mật khẩu
            PlayerInfo playerInfo = new PlayerInfo(username, email, passwordHash, characterId, exp, gold, gem);
            string json = JsonUtility.ToJson(playerInfo);
            dbRef.Child("players").Child(username).SetRawJsonValueAsync(json);

            if (profileTask.Exception != null)
            {
                if (messageText != null)
                {
                    messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                    messageText.text = "Failed to set user profile.";
                    messageText.gameObject.SetActive(true);
                }
                loadingScreen.SetActive(false);
            }
            else
            {
                var emailTask = user.SendEmailVerificationAsync();
                yield return new WaitUntil(() => emailTask.IsCompleted);

                if (emailTask.Exception != null)
                {
                    if (messageText != null)
                    {
                        messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                        messageText.text = "Failed to send verification email.";
                        messageText.gameObject.SetActive(true);
                    }
                    loadingScreen.SetActive(false);
                }
                else
                {
                    if (messageText != null)
                    {
                        messageText.color = successColor;  // Thiết lập màu xanh cho thông báo thành công
                        messageText.text = "Registration successful! Please verify your email.";
                        messageText.gameObject.SetActive(true);
                    }

                    // Lưu thông tin người dùng vào PlayerData
                    if (PlayerData.instance != null)
                    {
                        PlayerData.instance.playerId = user.UserId;
                        PlayerData.instance.username = username;
                        PlayerData.instance.email = email;
                        PlayerData.instance.passwordHash = passwordHash;
                        PlayerData.instance.characterId = characterId;
                        PlayerData.instance.exp = exp;
                        PlayerData.instance.gold = gold;
                        PlayerData.instance.gem = gem;
                    }
                    else
                    {
                        Debug.LogError("PlayerData.instance is null!");
                    }

                    // Xóa thông tin trong các trường nhập liệu
                    ResetInputFields();

                    // Ẩn màn hình loading sau khi đăng ký thành công
                    loadingScreen.SetActive(false);

                    // Hiển thị thông báo thành công trên popup đăng ký
                    if (registerPopup != null)
                    {
                        messageText.color = successColor;
                        messageText.text = "Registration successful! Please verify your email.";
                        messageText.gameObject.SetActive(true);
                    }

                    // Đợi 1 giây trước khi chuyển sang scene chọn nhân vật
                    yield return new WaitForSeconds(1);

                    // Chuyển sang scene chọn nhân vật sau khi đăng ký thành công
                    SceneManager.LoadScene("ChooseCharacter");


                }
            }
        }
    }

    private void ResetInputFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";
        emailInput.text = "";

        // Reset thông báo lỗi
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }

    public void OnCloseRegisterPopup()
    {
        ResetInputFields();
        // Reset thông báo đăng ký thành công
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
        // Ẩn popup đăng ký khi người dùng nhấn nút close
        if (registerPopup != null)
        {
            registerPopup.SetActive(false);
        }
    }

    private string ComputeHash(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
=======
}
>>>>>>> main
