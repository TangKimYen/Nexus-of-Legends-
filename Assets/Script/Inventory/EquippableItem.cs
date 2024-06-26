using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquippableType
{
    Armor,
    Boots,
    Buckler,
    Gauntle,
    Helmet,
    Necklace,
    Ring,
    Weapon,
}

[CreateAssetMenu]
public class EquippableItem : Item
{
    public int itemStrengthStat;
    public int itemBloodStat;
    public int itemMovementStat;
    public int itemAttackSpeedStat;
    public int itemIntellectStat;
    public int itemDefenseStat;
    [Space]
    public float itemStrengthPercent;
    public float itemBloodPercent;
    public float itemMovementPercent;
    public float itemAttackSpeedPercent;
    public float itemIntellectPercent;
    public float itemDefensePercent;
    [Space]
    public EquippableType equippableType;

}
