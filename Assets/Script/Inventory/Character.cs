using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;

    private void Awake()
    {
        inventory.OnItemRightClickedEvent += EquipFromInventory;
        equipmentPanel.OnItemRightClickedEvent += UnequipFromEquipPanel;

    }

    private void EquipFromInventory(Item item)
    {
        Debug.Log("EquipFromInventory được gọi với item: " + item);
        if (item is EquippableItem)
        {
            Equip((EquippableItem)item);
        }
    }

    private void UnequipFromEquipPanel(Item item)
    {
        if (item is EquippableItem)
        {
            Unequip((EquippableItem)item);
        }
    }

    public void Equip(EquippableItem item)
    {
        Debug.Log("Equip được gọi với item: " + item);
        if (inventory.RemoveItem(item))
        {
            Debug.Log("Item đã được gỡ khỏi inventory");
            EquippableItem previousItem;
            if (equipmentPanel.AddItem(item, out previousItem))
            {
                Debug.Log("Item đã được thêm vào equipment panel");
                if (previousItem != null)
                {
                    Debug.Log("Previous item: " + previousItem);
                    inventory.AddItem(previousItem);
                }
            }
            else
            {
                Debug.Log("Không thể thêm item vào equipment panel");
                inventory.AddItem(item);
            }
        }
    }

    public void Unequip(EquippableItem item)
    {
        if (!inventory.IsFull() && equipmentPanel.RemoveItem(item))
        {
            inventory.AddItem(item);
        }
    }

}
