using TMPro;
using UnityEngine;
using Firebase.Auth;
using System.Collections;

public class LogoutAction : MonoBehaviour
{
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần
    public TextMeshProUGUI usernameDisplayText; // Text để hiển thị username sau khi đăng xuất thành công
    public GameObject logoutPopup; // Popup đăng xuất

    private Color successColor;
    private Color errorColor;

    void Start()
    {
        // Chuyển đổi mã màu hex sang Color
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ
    }

    public void OnLogoutButtonClicked()
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
            PlayerPrefs.Save();

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

            // Cập nhật giao diện người dùng để phản ánh trạng thái đăng xuất
            // Ví dụ: quay lại màn hình đăng nhập hoặc tắt các chức năng của người dùng đã đăng nhập

            // Đợi 5 giây trước khi tắt popup
            yield return new WaitForSeconds(1);

            // Ẩn popup đăng xuất
            if (logoutPopup != null)
            {
                logoutPopup.SetActive(false);
            }
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
}