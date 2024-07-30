using System.Collections;
using TMPro;
using UnityEngine;
using Firebase.Auth;

public class LogoutAction : MonoBehaviour
{
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public TextMeshProUGUI usernameDisplayText; // Text để hiển thị username sau khi đăng xuất thành công
    public GameObject loginButton; // Nút đăng nhập
    public GameObject registerButton; // Nút đăng ký
    public GameObject logoutButton; // Nút đăng xuất
    public GameObject logoutConfirmationPopup; // Popup xác nhận đăng xuất

    private Color successColor;
    private Color errorColor;

    void Start()
    {
        // Chuyển đổi mã màu hex sang Color
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ

        // Ẩn nút đăng xuất ban đầu
        logoutButton.SetActive(false);

        // Ẩn popup xác nhận đăng xuất ban đầu
        if (logoutConfirmationPopup != null)
        {
            logoutConfirmationPopup.SetActive(false);
        }
    }

    public void OnLogoutButtonClicked()
    {
        // Hiển thị popup xác nhận đăng xuất
        if (logoutConfirmationPopup != null)
        {
            logoutConfirmationPopup.SetActive(true);
        }
    }

    public void ConfirmLogout()
    {
        StartCoroutine(LogoutUser());
    }

    private IEnumerator LogoutUser()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
        {
            auth.SignOut();

            // Xóa thông tin người dùng lưu trữ
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.DeleteKey("email");
            PlayerPrefs.DeleteKey("passwordHash");
            PlayerPrefs.DeleteKey("characterId");
            PlayerPrefs.DeleteKey("characterName");
            PlayerPrefs.DeleteKey("exp");
            PlayerPrefs.DeleteKey("gold");
            PlayerPrefs.DeleteKey("gem");
            PlayerPrefs.DeleteKey("level");
            PlayerPrefs.Save();
            // Cập nhật trạng thái đăng nhập
            PlayerData.instance.isLoggedIn = false;

            // Hiển thị thông báo đăng xuất thành công
            if (messageText != null)
            {
                messageText.color = successColor;  // Thiết lập màu xanh cho thông báo thành công
                messageText.text = "Logout successful!";
                messageText.gameObject.SetActive(true);
            }

            // Ẩn thông tin username sau khi đăng xuất
            if (usernameDisplayText != null)
            {
                usernameDisplayText.text = "";
                usernameDisplayText.gameObject.SetActive(false);
            }

            // Đợi 1 giây trước khi tắt popup
            yield return new WaitForSeconds(1);

            // Ẩn popup đăng xuất
            if (logoutConfirmationPopup != null)
            {
                logoutConfirmationPopup.SetActive(false);
            }
            // Hiển thị lại các nút đăng nhập và đăng ký
            loginButton.SetActive(true);
            registerButton.SetActive(true);

            // Ẩn nút đăng xuất
            logoutButton.SetActive(false);
        }
        else
        {
            // Nếu không có người dùng nào đang đăng nhập, hiển thị thông báo lỗi
            if (messageText != null)
            {
                messageText.color = errorColor;  // Thiết lập màu đỏ cho thông báo lỗi
                messageText.text = "No user is currently logged in.";
                messageText.gameObject.SetActive(true);
            }
        }

        yield return null;
    }

    public void CancelLogout()
    {
        // Ẩn popup xác nhận đăng xuất khi người dùng hủy bỏ đăng xuất
        if (logoutConfirmationPopup != null)
        {
            logoutConfirmationPopup.SetActive(false);
        }
    }
}