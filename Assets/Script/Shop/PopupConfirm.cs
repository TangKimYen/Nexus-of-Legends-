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
    [SerializeField] private GameObject insufficientGoldPopup; // Popup thông báo không đủ vàng
    [SerializeField] private Text insufficientGoldText; // Thêm Text để hiển thị thông báo không đủ vàng

    private string currentItemId;
    private float itemPrice;

    private void Start()
    {
        popupPanel.SetActive(false); // Ẩn pop-up khi bắt đầu
        confirmButton.onClick.AddListener(OnConfirmClick); // Đăng ký sự kiện cho nút xác nhận
        insufficientGoldPopup.SetActive(false); // Ẩn pop-up thông báo không đủ vàng khi bắt đầu
    }

    // Hiển thị pop-up với tên item
    public void ShowPopup(string itemName, string itemId, float price)
    {
        Debug.Log($"ShowPopup called with itemName: {itemName}, itemId: {itemId}");
        itemNameText.text = $"Are you sure you want to buy {itemName}?";
        currentItemId = itemId;
        itemPrice = price;
        popupPanel.SetActive(true);
        Debug.Log("Popup panel should now be active.");
    }

    // Xử lý logic khi nhấn nút confirm
    private void OnConfirmClick()
    {
        CheckPlayerGoldAndProceed();
    }

    // Đóng pop-up khi nhấn nút
    public void ClosePopup()
    {
        Debug.Log("Closing popup panel.");
        popupPanel.SetActive(false);
    }

    private void CheckPlayerGoldAndProceed()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance
            .GetReference("players")
            .Child(PlayerData.instance.username);

        reference.Child("gold").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    int playerGold = int.Parse(snapshot.Value.ToString());

                    if (playerGold >= itemPrice)
                    {
                        AddItemToInventory(currentItemId);
                        ClosePopup();
                    }
                    else
                    {
                        ShowInsufficientGoldPopup();
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
                Debug.LogError("Failed to retrieve player gold: " + task.Exception);
                ShowInsufficientGoldPopup("Failed to retrieve player gold.");
            }
        });
    }

    private void ShowInsufficientGoldPopup(string message = "You do not have enough gold to buy this item")
    {
        Debug.Log("Showing insufficient gold popup: " + message);
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
