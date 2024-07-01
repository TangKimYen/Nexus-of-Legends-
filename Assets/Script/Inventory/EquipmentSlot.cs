using UnityEngine;

public class EquipmentSlot : ItemSlot
{
    public EquippableType EquippableType;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = EquippableType.ToString() + " Slot";
    }

}
