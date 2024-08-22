//using System.Collections;
//using TMPro;
//using UnityEngine;
//using Firebase.Auth;
//using Firebase.Extensions;
//using Firebase.Database;
//using Firebase;
//using UnityEngine.SceneManagement;

//public class LoginAction : MonoBehaviour
//{
//    public TMP_InputField usernameInput;
//    public TMP_InputField passwordInput;
//    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
//    public TextMeshProUGUI usernameDisplayText; // Text để hiển thị username sau khi đăng nhập thành công
//    public GameObject loadingScreen;  // Hiển thị khi đang xử lý
//    public GameObject loginPopup; // Popup đăng nhập
//    public GameObject loginButton; // Nút đăng nhập
//    public GameObject registerButton; // Nút đăng ký
//    public GameObject logoutButton; // Nút đăng xuất
//    //public GameObject inventory;

//    private Color successColor;
//    private Color errorColor;
//    private DatabaseReference dbRef;

//    void Start()
//    {
//        // Chuyển đổi mã màu hex sang Color
//        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
//        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ
//        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
//        // Ẩn nút đăng xuất ban đầu
//        logoutButton.SetActive(false);
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
//        {
//            OnLoginButtonClicked();
//        }
//        if (Input.GetKeyDown(KeyCode.Tab))
//        {
//            SelectNextInputField();
//        }
//    }

//    public void OnLoginButtonClicked()
//    {
//        string username = usernameInput.text;
//        string password = passwordInput.text;

//        if (ValidateLogin(username, password))
//        {
//            StartCoroutine(LoginUser(username, password));
//        }
//        else
//        {
//            if (messageText != null)
//            {
//                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
//                messageText.text = "Invalid login information.";
//                messageText.gameObject.SetActive(true); // Hiển thị thông báo
//            }
//        }
//    }
//    private void SelectNextInputField()
//    {
//        if (usernameInput.isFocused)
//        {
//            passwordInput.Select();
//        }
//        else if (passwordInput.isFocused)
//        {
//            usernameInput.Select();
//        }
//    }

//    private bool ValidateLogin(string username, string password)
//    {
//        // Kiểm tra xem các trường có bị trống không
//        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
//        {
//            return false;
//        }

//        return true;
//    }

//    private IEnumerator LoginUser(string username, string password)
//    {
//        loadingScreen.SetActive(true);

//        // Lấy email từ username
//        var getEmailTask = dbRef.Child("players").Child(username).Child("emailInfo").GetValueAsync();
//        yield return new WaitUntil(() => getEmailTask.IsCompleted);

//        if (getEmailTask.Exception != null || !getEmailTask.Result.Exists)
//        {
//            messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
//            messageText.text = "Login failed: Invalid username.";
//            messageText.gameObject.SetActive(true);
//            loadingScreen.SetActive(false);
//        }
//        else
//        {
//            string email = getEmailTask.Result.Value.ToString();

//            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
//            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
//            yield return new WaitUntil(() => loginTask.IsCompleted);

//            if (loginTask.Exception != null)
//            {
//                FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
//                AuthError errorCode = (AuthError)firebaseException.ErrorCode;
//                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
//                messageText.text = "Login failed: Invalid username."/* + errorCode.ToString()*/;
//                messageText.gameObject.SetActive(true);
//                loadingScreen.SetActive(false);
//            }
//            else
//            {
//                AuthResult result = loginTask.Result;
//                FirebaseUser user = result.User;
//                //messageText.color = successColor;  // Thiết lập màu xanh cho thông báo thành công
//                //messageText.text = "Login successful! Welcome " + user.DisplayName;
//                //messageText.gameObject.SetActive(true);

//                // Lưu thông tin người dùng vào PlayerData
//                var getUserDataTask = dbRef.Child("players").Child(username).GetValueAsync();
//                yield return new WaitUntil(() => getUserDataTask.IsCompleted);

//                if (getUserDataTask.Exception != null || !getUserDataTask.Result.Exists)
//                {
//                    messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
//                    messageText.text = "Login failed: Unable to retrieve user data.";
//                    messageText.gameObject.SetActive(true);
//                    loadingScreen.SetActive(false);
//                }
//                else
//                {
//                    DataSnapshot snapshot = getUserDataTask.Result;

//                    if (snapshot.Exists)
//                    {
//                        PlayerData.instance.playerId = user.UserId;
//                        PlayerData.instance.username = username;
//                        PlayerData.instance.email = snapshot.Child("emailInfo").Value?.ToString();
//                        PlayerData.instance.passwordHash = snapshot.Child("passwordHash").Value?.ToString();
//                        PlayerData.instance.characterId = snapshot.Child("characterId").Value?.ToString();
//                        PlayerData.instance.characterName = snapshot.Child("characterName").Value?.ToString();
//                        PlayerData.instance.exp = snapshot.Child("exp").Value != null ? float.Parse(snapshot.Child("exp").Value.ToString()) : 0f;
//                        PlayerData.instance.gold = snapshot.Child("gold").Value != null ? float.Parse(snapshot.Child("gold").Value.ToString()) : 0f;
//                        PlayerData.instance.gem = snapshot.Child("gem").Value != null ? float.Parse(snapshot.Child("gem").Value.ToString()) : 0f;
//                        PlayerData.instance.level = snapshot.Child("level").Value != null ? int.Parse(snapshot.Child("level").Value.ToString()) : 1;

//                        // Tạo session ID và thời gian đăng nhập
//                        PlayerData.instance.sessionId = System.Guid.NewGuid().ToString();
//                        PlayerData.instance.loginTime = System.DateTime.UtcNow.ToString("o");

//                        // Cập nhật thông tin session và thời gian đăng nhập vào Firebase
//                        dbRef.Child("players").Child(username).Child("sessionId").SetValueAsync(PlayerData.instance.sessionId);
//                        dbRef.Child("players").Child(username).Child("loginTime").SetValueAsync(PlayerData.instance.loginTime);

//                        // Đặt cờ đánh dấu người chơi đã đăng nhập
//                        PlayerData.instance.isLoggedIn = true;

//                        // Xóa các trường nhập liệu sau khi đăng nhập thành công
//                        ResetInputFields();

//                        // Thiết lập userName và tải mục từ Firebase cho Inventory
//                        //if (inventory != null)
//                        //{
//                        //    inventory.userName = username;
//                        //    inventory.LoadItemsFromFirebase(); // Tải các mục từ Firebase
//                        //}

//                        // Ẩn màn hình loading sau khi đăng nhập thành công
//                        loadingScreen.SetActive(false);

//                        // Đợi 1 giây trước khi tắt popup đăng nhập
//                        yield return new WaitForSeconds(1);
//                        // Xóa thông báo sau khi tắt popup đăng nhập
//                        if (messageText != null)
//                        {
//                            messageText.text = "";
//                            messageText.gameObject.SetActive(false);
//                        }

//                        // Hiển thị username sau khi đăng nhập thành công
//                        if (usernameDisplayText != null)
//                        {
//                            usernameDisplayText.text = "Player: " + username;
//                            usernameDisplayText.gameObject.SetActive(true);
//                        }
//                        // Ẩn popup đăng nhập khi người dùng nhấn nút close
//                        if (loginPopup != null)
//                        {
//                            loginPopup.SetActive(false);
//                        }
//                        // Ẩn các nút đăng nhập và đăng ký
//                        loginButton.SetActive(false);
//                        registerButton.SetActive(false);

//                        // Hiển thị nút đăng xuất
//                        logoutButton.SetActive(true);
//                    }
//                    else
//                    {
//                        messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
//                        messageText.text = "Login failed: User data does not exist.";
//                        messageText.gameObject.SetActive(true);
//                        loadingScreen.SetActive(false);
//                    }
//                }
//            }
//        }
//    }

//    private void ResetInputFields()
//    {
//        usernameInput.text = "";
//        passwordInput.text = "";
//    }

//    public void OnCloseLoginPopup()
//    {
//        // Reset thông báo đăng nhập thành công hoặc thất bại
//        if (messageText != null)
//        {
//            messageText.text = "";
//            messageText.gameObject.SetActive(false);
//        }

//        // Reset các trường nhập liệu
//        ResetInputFields();

//        // Ẩn popup đăng nhập khi người dùng nhấn nút close
//        if (loginPopup != null)
//        {
//            loginPopup.SetActive(false);
//        }
//    }
//}
using System.Collections;
using TMPro;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using Firebase;
using UnityEngine.SceneManagement;

public class LoginAction : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public TextMeshProUGUI usernameDisplayText; // Text để hiển thị username sau khi đăng nhập thành công
    public GameObject loadingScreen;  // Hiển thị khi đang xử lý
    public GameObject loginPopup; // Popup đăng nhập
    public GameObject loginButton; // Nút đăng nhập
    public GameObject registerButton; // Nút đăng ký
    public GameObject logoutButton; // Nút đăng xuất
    private string currentSessionId;
    private Color successColor;
    private Color errorColor;
    private DatabaseReference dbRef;

    void Start()
    {
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        logoutButton.SetActive(false);
        // Kiểm tra nếu người dùng bị đăng xuất do đăng nhập từ nơi khác
        if (PlayerData.instance.wasLoggedOutByOtherLogin)
        {
            PlayerData.instance.wasLoggedOutByOtherLogin = false; // Reset lại cờ

            // Hiển thị popup đăng nhập và thông báo
            if (loginPopup != null)
            {
                loginPopup.SetActive(true); // Hiển thị popup đăng nhập
            }

            if (messageText != null)
            {
                messageText.color = errorColor;
                messageText.text = "You have been logged out because someone else logged into your account.";
                messageText.gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnLoginButtonClicked();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextInputField();
        }

        // Kiểm tra session ID định kỳ để phát hiện nếu có người khác đăng nhập
        if (PlayerData.instance.isLoggedIn)
        {
            StartCoroutine(CheckSessionValidity(PlayerData.instance.username));
        }
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

    private void SelectNextInputField()
    {
        if (usernameInput.isFocused)
        {
            passwordInput.Select();
        }
        else if (passwordInput.isFocused)
        {
            usernameInput.Select();
        }
    }

    private bool ValidateLogin(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return false;
        }
        return true;
    }

    private IEnumerator LoginUser(string username, string password)
    {
        loadingScreen.SetActive(true);

        var getEmailTask = dbRef.Child("players").Child(username).Child("emailInfo").GetValueAsync();
        yield return new WaitUntil(() => getEmailTask.IsCompleted);

        if (getEmailTask.Exception != null || !getEmailTask.Result.Exists)
        {
            messageText.color = errorColor;
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
                messageText.color = errorColor;
                messageText.text = "Login failed: " + errorCode.ToString();
                messageText.gameObject.SetActive(true);
                loadingScreen.SetActive(false);
            }
            else
            {
                FirebaseUser user = loginTask.Result.User;

                var getUserDataTask = dbRef.Child("players").Child(username).GetValueAsync();
                yield return new WaitUntil(() => getUserDataTask.IsCompleted);

                if (getUserDataTask.Exception != null || !getUserDataTask.Result.Exists)
                {
                    messageText.color = errorColor;
                    messageText.text = "Login failed: Unable to retrieve user data.";
                    messageText.gameObject.SetActive(true);
                    loadingScreen.SetActive(false);
                }
                else
                {
                    DataSnapshot snapshot = getUserDataTask.Result;

                    if (snapshot.Exists)
                    {
                        PlayerData.instance.playerId = user.UserId;
                        PlayerData.instance.username = username;
                        PlayerData.instance.email = snapshot.Child("emailInfo").Value?.ToString();
                        PlayerData.instance.passwordHash = snapshot.Child("passwordHash").Value?.ToString();
                        PlayerData.instance.characterId = snapshot.Child("characterId").Value?.ToString();
                        PlayerData.instance.characterName = snapshot.Child("characterName").Value?.ToString();
                        PlayerData.instance.exp = snapshot.Child("exp").Value != null ? float.Parse(snapshot.Child("exp").Value.ToString()) : 0f;
                        PlayerData.instance.gold = snapshot.Child("gold").Value != null ? float.Parse(snapshot.Child("gold").Value.ToString()) : 0f;
                        PlayerData.instance.gem = snapshot.Child("gem").Value != null ? float.Parse(snapshot.Child("gem").Value.ToString()) : 0f;
                        PlayerData.instance.level = snapshot.Child("level").Value != null ? int.Parse(snapshot.Child("level").Value.ToString()) : 1;

                        // Tạo session ID và thời gian đăng nhập
                        PlayerData.instance.sessionId = System.Guid.NewGuid().ToString();
                        PlayerData.instance.loginTime = System.DateTime.UtcNow.ToString("o");

                        // Lưu session ID hiện tại
                        currentSessionId = PlayerData.instance.sessionId;

                        // Cập nhật thông tin session và thời gian đăng nhập vào Firebase
                        dbRef.Child("players").Child(username).Child("sessionId").SetValueAsync(PlayerData.instance.sessionId);
                        dbRef.Child("players").Child(username).Child("loginTime").SetValueAsync(PlayerData.instance.loginTime);

                        PlayerData.instance.isLoggedIn = true;

                        messageText.color = successColor;
                        messageText.text = "Login successful! Welcome " + username + ".";
                        messageText.gameObject.SetActive(true);

                        ResetInputFields();
                        loadingScreen.SetActive(false);

                        yield return new WaitForSeconds(1);

                        if (messageText != null)
                        {
                            messageText.text = "";
                            messageText.gameObject.SetActive(false);
                        }

                        if (usernameDisplayText != null)
                        {
                            usernameDisplayText.text = "Player: " + username;
                            usernameDisplayText.gameObject.SetActive(true);
                        }

                        if (loginPopup != null)
                        {
                            loginPopup.SetActive(false);
                        }

                        loginButton.SetActive(false);
                        registerButton.SetActive(false);
                        logoutButton.SetActive(true);
                    }
                    else
                    {
                        messageText.color = errorColor;
                        messageText.text = "Login failed: User data does not exist.";
                        messageText.gameObject.SetActive(true);
                        loadingScreen.SetActive(false);
                    }
                }
            }
        }
    }

    private void ResetInputFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
    }

    private IEnumerator CheckSessionValidity(string username)
    {
        var getSessionTask = dbRef.Child("players").Child(username).Child("sessionId").GetValueAsync();
        yield return new WaitUntil(() => getSessionTask.IsCompleted);

        if (getSessionTask.Exception == null && getSessionTask.Result.Exists)
        {
            string latestSessionId = getSessionTask.Result.Value.ToString();
            if (latestSessionId != currentSessionId)
            {
                // Session ID đã thay đổi, có người khác đã đăng nhập vào tài khoản này
                PlayerData.instance.wasLoggedOutByOtherLogin = true;
                PlayerData.instance.CheckLogoutReason(); // Kiểm tra lý do đăng xuất
                LogoutUser();
            }
        }
    }

    public void LogoutUser()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        PlayerData.instance.isLoggedIn = false;
        PlayerData.instance.wasLoggedOutByOtherLogin = true; // Đặt cờ đăng xuất bởi đăng nhập từ nơi khác

        // Chuyển về màn hình chính
        SceneManager.LoadScene("TitleScreen");
    }

    public void OnCloseLoginPopup()
    {
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }

        ResetInputFields();

        if (loginPopup != null)
        {
            loginPopup.SetActive(false);
        }
    }
}
