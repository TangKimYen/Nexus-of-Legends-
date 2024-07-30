using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{
    public Character character; // Thêm thuộc tính này
    public ItemDatabase itemDatabase;
    [SerializeField] Transform equipmentSlotsParent;
    [SerializeField] public EquipmentSlot[] equipmentSlots;
    [SerializeField] ItemTooltip tooltip;
    public event Action<Item> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }
    private void OnValidate()
    {
        equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>();
        
    }

    public bool AddItem(EquippableItem item, out EquippableItem previousItem)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].EquippableType == item.equippableType)
            {
                previousItem = (EquippableItem)equipmentSlots[i].Item;
                equipmentSlots[i].Item = item;
                return true;
            }
        }
        previousItem = null;
        return false;
    }

    public bool RemoveItem(EquippableItem item)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].Item == item)
            {
                equipmentSlots[i].Item = null;
                return true;
            }
        }
        return false;
    }

    private void ShowTooltip(Item item)
    {
        if (item != null && item is EquippableItem equippableItem)
        {
            tooltip.ShowTooltip(equippableItem);
        }
    }

    private void HideTooltip(Item item)
    {
        tooltip.HideTooltip();
    }


}
