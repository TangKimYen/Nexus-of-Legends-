using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Thêm thư viện này để sử dụng SceneManager
using Firebase.Auth;

public class LogoutInMainLobby : MonoBehaviour
{
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public GameObject logoutConfirmationPopup; // Popup xác nhận đăng xuất
    public string titleScreenSceneName = "TitleScreen"; // Tên của scene màn hình title screen

    private Color successColor;
    private Color errorColor;

    void Start()
    {
        // Chuyển đổi mã màu hex sang Color
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ

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

            // Xóa thông tin người dùng lưu trữ (nếu cần)
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

            // Reset PlayerData instance
            if (PlayerData.instance != null)
            {
                PlayerData.instance.LogoutAndReturnToTitleScreen();
            }

            // Hiển thị thông báo đăng xuất thành công (nếu cần)
            if (messageText != null)
            {
                messageText.color = successColor;  // Thiết lập màu xanh cho thông báo thành công
                messageText.text = "Logout successful!";
                messageText.gameObject.SetActive(true);
            }

            // Đợi 1 giây trước khi tắt popup
            yield return new WaitForSeconds(1);

            // Ẩn popup đăng xuất
            if (logoutConfirmationPopup != null)
            {
                logoutConfirmationPopup.SetActive(false);
            }

            // Load lại màn hình title screen
            SceneManager.LoadScene(titleScreenSceneName);
        }
        else
        {
            // Nếu không có người dùng nào đang đăng nhập, hiển thị thông báo lỗi (nếu cần)
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