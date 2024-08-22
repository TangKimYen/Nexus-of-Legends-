using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirm : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject insufficientGoldPopup; // Popup thông báo không đủ vàng hoặc gem
    [SerializeField] private Text insufficientGoldText; // Thêm Text để hiển thị thông báo không đủ vàng hoặc gem

    private string currentItemId;
    private float itemPrice;
    private bool isGemPurchase; // Biến để lưu trạng thái mua bằng gem

    private void Start()
    {
        popupPanel.SetActive(false); // Ẩn pop-up khi bắt đầu
        confirmButton.onClick.AddListener(OnConfirmClick); // Đăng ký sự kiện cho nút xác nhận
        insufficientGoldPopup.SetActive(false); // Ẩn pop-up thông báo không đủ vàng/gem khi bắt đầu
    }

    // Hiển thị pop-up với tên item
    public void ShowPopup(string itemName, string itemId, float price, bool isGem)
    {
        Debug.Log($"ShowPopup called with itemName: {itemName}, itemId: {itemId}");
        itemNameText.text = $"Are you sure you want to buy {itemName}?";
        currentItemId = itemId;
        itemPrice = price;
        isGemPurchase = isGem; // Lưu lại trạng thái mua bằng gem hay không
        popupPanel.SetActive(true);
        Debug.Log("Popup panel should now be active.");
    }

    // Xử lý logic khi nhấn nút confirm
    private void OnConfirmClick()
    {
        CheckPlayerCurrencyAndProceed();
    }

    // Đóng pop-up khi nhấn nút
    public void ClosePopup()
    {
        Debug.Log("Closing popup panel.");
        popupPanel.SetActive(false);
    }

    private void CheckPlayerCurrencyAndProceed()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance
            .GetReference("players")
            .Child(PlayerData.instance.username);

        string currencyType = isGemPurchase ? "gem" : "gold"; // Kiểm tra loại tiền tệ (gem hoặc vàng)

        reference.Child(currencyType).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    float playerCurrency = float.Parse(snapshot.Value.ToString());

                    if (playerCurrency >= itemPrice)
                    {
                        DeductPlayerCurrency(playerCurrency - itemPrice, currencyType);
                        AddItemToInventory(currentItemId);
                        ClosePopup();
                    }
                    else
                    {
                        ShowInsufficientGoldPopup($"You do not have enough {currencyType} to buy this item.");
                    }
                }
                else
                {
                    Debug.LogError("Player data not found.");
                    ShowInsufficientGoldPopup("Player data not found.");
                }
            }
            else
            {
                Debug.LogError("Failed to retrieve player currency: " + task.Exception);
                ShowInsufficientGoldPopup("Failed to retrieve player currency.");
            }
        });
    }

    private void DeductPlayerCurrency(float newCurrencyAmount, string currencyType)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance
            .GetReference("players")
            .Child(PlayerData.instance.username)
            .Child(currencyType);

        reference.SetValueAsync(newCurrencyAmount).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log($"{currencyType} deducted successfully.");
            }
            else
            {
                Debug.LogError($"Failed to deduct {currencyType}: " + task.Exception);
            }
        });
    }

    private void ShowInsufficientGoldPopup(string message = "You do not have enough currency to buy this item")
    {
        Debug.Log("Showing insufficient currency popup: " + message);
        insufficientGoldText.text = message; // Cập nhật văn bản của popup
        insufficientGoldPopup.SetActive(true);
    }

    private void AddItemToInventory(string itemId)
    {
        ItemData newItem = new ItemData { itemId = itemId, isActive = true };

        DatabaseReference reference = FirebaseDatabase.DefaultInstance
            .GetReference("Inventory")
            .Child(PlayerData.instance.username)
            .Child(itemId);

        string json = JsonUtility.ToJson(newItem);
        reference.SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Item added to inventory successfully.");
            }
            else
            {
                Debug.LogError("Failed to add item to inventory: " + task.Exception);
            }
        });
    }
}
