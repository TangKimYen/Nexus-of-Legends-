using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] ItemTooltip tooltip;

    public event Action<Item> OnRightClickEvent;
    public event Action<Item> OnLeftDoubleClickEvent; // Thêm sự kiện cho double-click chuột trái

    private Item _item;
    [SerializeField] private SellItemPopup popupManager;
    public Item Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
            {
                image.enabled = false;
            }
            else
            {
                image.sprite = _item.icon;
                image.enabled = true;
            }
        }
    }

    protected virtual void OnValidate()
    {
        if (image == null)
            image = GetComponent<Image>();

        if (tooltip == null)
            tooltip = FindObjectOfType<ItemTooltip>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (Item != null && OnRightClickEvent != null)
            {
                OnRightClickEvent(Item);
            }
        }
        if (Item != null)
        {
            if (popupManager != null)
            {
                Debug.Log($"Showing popup for item: {Item.itemName}, itemId: {Item.itemId}");
                popupManager.ShowPopup(Item.itemName, Item.itemId, Item.itemCoin);
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
        if (Item is EquippableItem equippableItem)
        {
            tooltip.ShowTooltip(equippableItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }
}
