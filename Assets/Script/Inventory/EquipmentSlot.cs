using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    public EquippableType EquippableType;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = EquippableType.ToString() + " Slot";
    }

    public override bool CanReciveItem(Item item)
    {
        if(item == null)
        {
            return true;
        }
        EquippableItem equippableItem = item as EquippableItem;
        return equippableItem != null && equippableItem.equippableType == EquippableType;
    }
}
