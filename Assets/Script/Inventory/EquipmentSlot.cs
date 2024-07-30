using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : ItemSlot, IPointerEnterHandler, IPointerExitHandler
{
    public EquippableType EquippableType;
    public delegate void ItemEventHandler(Item item);
    public event ItemEventHandler OnPointerEnterEvent;
    public event ItemEventHandler OnPointerExitEvent;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = EquippableType.ToString() + " Slot";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Item != null)
        {
            OnPointerEnterEvent?.Invoke(Item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Item != null)
        {
            OnPointerExitEvent?.Invoke(Item);
        }
    }

}
