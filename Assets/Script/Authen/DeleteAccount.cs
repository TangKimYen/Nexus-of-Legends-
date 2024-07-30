using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeleteAccountAction : MonoBehaviour
{
    public Button deleteAccountButton;
    public TextMeshProUGUI messageText;  // Dùng để hiển thị thông báo, nếu cần

    private Color successColor;
    private Color errorColor;

    void Start()
    {
        // Chuyển đổi mã màu hex sang Color
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ

        if (deleteAccountButton != null)
        {
            deleteAccountButton.onClick.AddListener(OnDeleteAccountButtonClicked);
        }
    }

    private void OnDeleteAccountButtonClicked()
    {
        if (PlayerData.instance != null)
        {
            PlayerData.instance.DeleteAccount();
            SceneManager.LoadScene("TitleScreen");
        }
        else
        {
            if (messageText != null)
            {
                messageText.color = errorColor;
                messageText.text = "Error: Player data instance not found.";
                messageText.gameObject.SetActive(true);
            }
        }
    }
}