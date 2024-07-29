using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] Transform profileSlotsParent;
    [SerializeField] public EquipmentSlot[] profileSlots;
    [SerializeField] ItemTooltip tooltip;

    private void OnValidate()
    {
        profileSlots = profileSlotsParent.GetComponentsInChildren<EquipmentSlot>();
    }

    private void Start()
    {
        foreach (var slot in profileSlots)
        {
            slot.OnPointerEnterEvent += ShowTooltip;
            slot.OnPointerExitEvent += HideTooltip;
        }
    }

    public void UpdateProfilePanel(EquipmentSlot[] equippedSlots)
    {
        for (int i = 0; i < profileSlots.Length; i++)
        {
            if (i < equippedSlots.Length)
            {
                profileSlots[i].Item = equippedSlots[i].Item;
            }
            else
            {
                profileSlots[i].Item = null;
            }
        }
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
