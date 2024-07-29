using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Text coin;
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick called");
        if (Item != null)
        {
            if (popupManager != null)
            {
                Debug.Log($"Showing popup for item: {Item.itemName}, itemId: {Item.itemId}");
                popupManager.ShowPopup(Item.itemName, Item.itemId);
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