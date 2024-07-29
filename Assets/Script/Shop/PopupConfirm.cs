using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirm : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Text itemNameText;
    [SerializeField] private Button confirmButton;

    private string currentItemId;

    private void Start()
    {
        popupPanel.SetActive(false); // Ẩn pop-up khi bắt đầu
        confirmButton.onClick.AddListener(OnConfirmClick); // Đăng ký sự kiện cho nút xác nhận
    }

    // Hiển thị pop-up với tên item
    public void ShowPopup(string itemName, string itemId)
    {
        Debug.Log($"ShowPopup called with itemName: {itemName}, itemId: {itemId}");
        itemNameText.text = $"Are you sure you want to buy {itemName}?";
        currentItemId = itemId;
        popupPanel.SetActive(true);
        Debug.Log("Popup panel should now be active.");
    }

    // Xử lý logic khi nhấn nút confirm
    private void OnConfirmClick()
    {
        if (!string.IsNullOrEmpty(currentItemId))
        {
            AddItemToInventory(currentItemId);
        }
        ClosePopup();
    }

    // Đóng pop-up khi nhấn nút
    public void ClosePopup()
    {
        Debug.Log("Closing popup panel.");
        popupPanel.SetActive(false);
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
