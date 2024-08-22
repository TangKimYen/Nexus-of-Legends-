using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text coin;
    [SerializeField] private Image currencyIcon; // Thêm Image cho icon kế bên giá
    [SerializeField] private Sprite gemIcon;    // Icon cho gem
    [SerializeField] private Sprite goldIcon;   // Icon cho gold
    [SerializeField] private PopupConfirm popupManager;
    [SerializeField] private ItemTooltip tooltip;

    public Item Item { get; private set; }

    private void Awake()
    {
        // Đảm bảo tooltip và popupManager được gán trong Awake
        if (tooltip == null)
        {
            tooltip = FindObjectOfType<ItemTooltip>();
            if (tooltip == null)
            {
                Debug.LogError("ItemTooltip không tìm thấy trong cảnh.");
            }
        }

        if (popupManager == null)
        {
            popupManager = FindObjectOfType<PopupConfirm>();
            if (popupManager == null)
            {
                Debug.LogError("PopupManager không tìm thấy trong cảnh.");
            }
        }
    }

    public void SetItem(Item newItem)
    {
        Item = newItem;
        if (Item != null)
        {
            icon.sprite = newItem.icon;
            coin.text = newItem.itemCoin.ToString();
            icon.enabled = true;
            coin.enabled = true;

            // Hiển thị icon gem hoặc gold dựa vào thuộc tính isGem
            if (newItem.isGem)
            {
                currencyIcon.sprite = gemIcon;
            }
            else
            {
                currencyIcon.sprite = goldIcon;
            }
            currencyIcon.enabled = true;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        Item = null;
        icon.sprite = null;
        coin.text = string.Empty;
        icon.enabled = false;
        coin.enabled = false;
        currencyIcon.enabled = false; // Ẩn icon nếu slot trống
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick called");
        if (Item != null)
        {
            if (popupManager != null)
            {
                Debug.Log($"Showing popup for item: {Item.itemName}, itemId: {Item.itemId}");
                popupManager.ShowPopup(Item.itemName, Item.itemId, Item.itemCoin, Item.isGem);
            }
            else
            {
                Debug.LogError("PopupManager chưa được gán.");
            }
        }
        else
        {
            Debug.LogError("Item là null.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null && Item is EquippableItem equippableItem)
        {
            tooltip.ShowTooltip(equippableItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.HideTooltip();
        }
    }
}
