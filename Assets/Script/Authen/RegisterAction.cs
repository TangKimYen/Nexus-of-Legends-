using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
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

public class RegisterAction : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField fullnameInput;
    public TMP_InputField emailInput;
    public TextMeshProUGUI messageText;  // DÃ¹ng Ä‘á»ƒ hiá»ƒn thá»‹ thÃ´ng bÃ¡o, náº¿u cáº§n
    public TextMeshProUGUI usernameDisplayText; // Text Ä‘á»ƒ hiá»ƒn thá»‹ username sau khi Ä‘Äƒng kÃ½ thÃ nh cÃ´ng
    public GameObject loadingScreen;  // Hiá»ƒn thá»‹ khi Ä‘ang xá»­ lÃ½
    public GameObject registerPopup; // Popup Ä‘Äƒng kÃ½

    // Äá»‹nh nghÄ©a cÃ¡c mÃ u tÃ¹y chá»‰nh báº±ng mÃ£ mÃ u hex
    private Color successColor;
    private Color errorColor;

    DatabaseReference dbRef;

    void Start()
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

    private IEnumerator CheckUsernameAndRegister(string username, string email, string password)
    {
        loadingScreen.SetActive(true);

        var usernameCheckTask = dbRef.Child("players").Child(username).GetValueAsync();
        yield return new WaitUntil(() => usernameCheckTask.IsCompleted);

        if (usernameCheckTask.Exception != null)
        {
            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiáº¿t láº­p mÃ u Ä‘á» cho thÃ´ng bÃ¡o lá»—i
                messageText.text = "Error checking username.";
                messageText.gameObject.SetActive(true); // Hiá»ƒn thá»‹ thÃ´ng bÃ¡o
            }
            loadingScreen.SetActive(false);
        }
        else if (usernameCheckTask.Result.Exists)
        {
            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiáº¿t láº­p mÃ u Ä‘á» cho thÃ´ng bÃ¡o lá»—i
                messageText.text = "Username is already taken.";
                messageText.gameObject.SetActive(true); // Hiá»ƒn thá»‹ thÃ´ng bÃ¡o
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
                messageText.color = errorColor;  // Thiáº¿t láº­p mÃ u Ä‘á» cho thÃ´ng bÃ¡o lá»—i
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

            string characterId = ""; // Äá»ƒ trá»‘ng ban Ä‘áº§u
            float exp = 0;
            float gold = 0;
            float gem = 0;
            string passwordHash = ComputeHash(password); // BÄƒm máº­t kháº©u
            PlayerInfo playerInfo = new PlayerInfo(username, email, passwordHash, characterId, exp, gold, gem);
            string json = JsonUtility.ToJson(playerInfo);
            dbRef.Child("players").Child(username).SetRawJsonValueAsync(json);

            if (profileTask.Exception != null)
            {
                if (messageText != null)
                {
                    messageText.color = errorColor;  // Thiáº¿t láº­p mÃ u Ä‘á» cho thÃ´ng bÃ¡o lá»—i
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
                        messageText.color = errorColor;  // Thiáº¿t láº­p mÃ u Ä‘á» cho thÃ´ng bÃ¡o lá»—i
                        messageText.text = "Failed to send verification email.";
                        messageText.gameObject.SetActive(true);
                    }
                    loadingScreen.SetActive(false);
                }
                else
                {
                    if (messageText != null)
                    {
                        messageText.color = successColor;  // Thiáº¿t láº­p mÃ u xanh cho thÃ´ng bÃ¡o thÃ nh cÃ´ng
                        messageText.text = "Registration successful! Please verify your email.";
                        messageText.gameObject.SetActive(true);
                    }

                    // LÆ°u thÃ´ng tin ngÆ°á»i dÃ¹ng vÃ o PlayerData
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

                    // XÃ³a thÃ´ng tin trong cÃ¡c trÆ°á»ng nháº­p liá»‡u
                    ResetInputFields();

                    // áº¨n mÃ n hÃ¬nh loading sau khi Ä‘Äƒng kÃ½ thÃ nh cÃ´ng
                    loadingScreen.SetActive(false);

                    // Hiá»ƒn thá»‹ thÃ´ng bÃ¡o thÃ nh cÃ´ng trÃªn popup Ä‘Äƒng kÃ½
                    if (registerPopup != null)
                    {
                        messageText.color = successColor;
                        messageText.text = "Registration successful! Please verify your email.";
                        messageText.gameObject.SetActive(true);
                    }

                    // Äá»£i 1 giÃ¢y trÆ°á»›c khi chuyá»ƒn sang scene chá»n nhÃ¢n váº­t
                    yield return new WaitForSeconds(1);

                    // Chuyá»ƒn sang scene chá»n nhÃ¢n váº­t sau khi Ä‘Äƒng kÃ½ thÃ nh cÃ´ng
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

        // Reset thÃ´ng bÃ¡o lá»—i
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }

    public void OnCloseRegisterPopup()
    {
        ResetInputFields();
        // Reset thÃ´ng bÃ¡o Ä‘Äƒng kÃ½ thÃ nh cÃ´ng
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
        // áº¨n popup Ä‘Äƒng kÃ½ khi ngÆ°á»i dÃ¹ng nháº¥n nÃºt close
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
