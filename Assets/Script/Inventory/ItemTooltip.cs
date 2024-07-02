using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.EventSystems;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemSlotText;
    [SerializeField] Text ItemStatsText;

    private StringBuilder sb = new StringBuilder();

    public void ShowTooltip(EquippableItem item)
    {
        ItemNameText.text = item.itemName;
        ItemSlotText.text = item.equippableType.ToString();

        sb.Length = 0; 
        AddStat(item.itemStrengthStat, "Strength");
        AddStat(item.itemIntellectStat, "Intellect");
        AddStat(item.itemDefenseStat, "Defense");
        AddStat(item.itemBloodStat, "Blood");
        AddStat(item.itemMovementStat, "Movement");
        AddStat(item.itemAttackSpeedStat, "Attack Speed");

        AddStat(item.itemStrengthPercent, "Strength", isPercent: true);
        AddStat(item.itemIntellectPercent, "Intellect", isPercent: true);
        AddStat(item.itemDefensePercent, "Defense", isPercent: true);
        AddStat(item.itemBloodPercent, "Blood", isPercent: true);
        AddStat(item.itemMovementPercent, "Movement", isPercent: true);
        AddStat(item.itemAttackSpeedPercent, "Attack Speed", isPercent: true);

        ItemStatsText.text = sb.ToString();

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void AddStat(float value, string statName, bool isPercent = false)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (value > 0)
                sb.Append("+");

            if (isPercent)
            {
                sb.Append(value * 100);
                sb.Append("% ");
            }
            else
            {
                sb.Append(value);
                sb.Append(" ");
            }
            sb.Append(statName);
        }
        
    }

}
