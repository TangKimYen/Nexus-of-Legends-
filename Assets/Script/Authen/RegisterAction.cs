using System.Collections;
using System.Security.Cryptography;
using System.Text;
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
    public int level;

    public PlayerInfo(string usernameInfo, string emailInfo, string passwordHash, string characterId, float exp, float gold, float gem, int level)
    {
        this.usernameInfo = usernameInfo;
        this.emailInfo = emailInfo;
        this.passwordHash = passwordHash;
        this.characterId = characterId;
        this.exp = exp;
        this.gold = gold;
        this.gem = gem;
        this.level = level;
    }
}

public class RegisterAction : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField emailInput;
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public TextMeshProUGUI usernameDisplayText; // Text để hiển thị username sau khi đăng ký thành công
    public GameObject loadingScreen;  // Hiển thị khi đang xử lý
    public GameObject registerPopup; // Popup đăng ký

    private Color successColor;
    private Color errorColor;
    DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        ColorUtility.TryParseHtmlString("#007213", out successColor);
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);
    }

    public void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;
        string email = emailInput.text;

        string errorMessage;
        if (ValidateRegister(username, password, confirmPassword, email, out errorMessage))
        {
            StartCoroutine(CheckUsernameAndRegister(username, email, password));
        }
        else
        {
            if (messageText != null)
            {
                messageText.color = errorColor;
                messageText.text = errorMessage;
                messageText.gameObject.SetActive(true);
            }
        }
    }

    private bool ValidateRegister(string username, string password, string confirmPassword, string email, out string errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(email))
        {
            errorMessage = "All fields are required.";
            return false;
        }

        if (username.Length > 16)
        {
            errorMessage = "Username must be 16 characters or less.";
            return false;
        }

        if (password != confirmPassword)
        {
            errorMessage = "Passwords do not match.";
            return false;
        }

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
        {
            errorMessage = "Invalid email format.";
            return false;
        }

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
                messageText.color = errorColor;
                messageText.text = "Error checking username.";
                messageText.gameObject.SetActive(true);
            }
            loadingScreen.SetActive(false);
        }
        else if (usernameCheckTask.Result.Exists)
        {
            if (messageText != null)
            {
                messageText.color = errorColor;
                messageText.text = "Username is already taken.";
                messageText.gameObject.SetActive(true);
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
                messageText.color = errorColor;
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

            string characterId = "";
            float exp = 0;
            float gold = 0;
            float gem = 0;
            int level = 1; // Level ban đầu là 1
            string passwordHash = ComputeHash(password);
            PlayerInfo playerInfo = new PlayerInfo(username, email, passwordHash, characterId, exp, gold, gem, level);
            string json = JsonUtility.ToJson(playerInfo);
            dbRef.Child("players").Child(username).SetRawJsonValueAsync(json);

            if (profileTask.Exception != null)
            {
                if (messageText != null)
                {
                    messageText.color = errorColor;
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
                        messageText.color = errorColor;
                        messageText.text = "Failed to send verification email.";
                        messageText.gameObject.SetActive(true);
                    }
                    loadingScreen.SetActive(false);
                }
                else
                {
                    if (messageText != null)
                    {
                        messageText.color = successColor;
                        messageText.text = "Registration successful! Please verify your email.";
                        messageText.gameObject.SetActive(true);
                    }

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
                        PlayerData.instance.level = level;
                    }
                    else
                    {
                        Debug.LogError("PlayerData.instance is null!");
                    }

                    ResetInputFields();
                    loadingScreen.SetActive(false);
                    yield return new WaitForSeconds(1);
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