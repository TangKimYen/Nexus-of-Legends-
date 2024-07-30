using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class SellItemPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Button confirmButton;

    private Inventory inventory;
    private string currentItemId;
    private float currentItemPrice;

    private void Start()
    {
        popupPanel.SetActive(false); // Ẩn pop-up khi bắt đầu
        confirmButton.onClick.AddListener(OnConfirmClick);

        inventory = FindObjectOfType<Inventory>();
    }

    // Hiển thị pop-up với tên item
    public void ShowPopup(string itemName, string itemId, float price)
    {
        itemNameText.text = $"Are you sure you want to sell {itemName}?";
        currentItemId = itemId;
        currentItemPrice = price;
        popupPanel.SetActive(true);
    }

    // Đóng pop-up khi nhấn nút
    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    private void OnConfirmClick()
    {
        if (inventory != null && !string.IsNullOrEmpty(currentItemId))
        {
            inventory.SellItem(currentItemId, currentItemPrice);
        }
        ClosePopup();
    }
}
