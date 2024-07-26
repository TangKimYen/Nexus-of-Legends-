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
    public int price;
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

    public void Equip(Character c)
    {
        if (itemStrengthStat !=  0)
            c.Strength.AddModifier(new StatModifier(itemStrengthStat, StatModType.Flat, this));
        if (itemBloodStat != 0)
            c.Blood.AddModifier(new StatModifier(itemBloodStat, StatModType.Flat, this));
        if (itemMovementStat != 0)
            c.Movement.AddModifier(new StatModifier(itemMovementStat, StatModType.Flat, this));
        if (itemAttackSpeedStat != 0)
            c.AttackSpeed.AddModifier(new StatModifier(itemAttackSpeedStat, StatModType.Flat, this));
        if (itemIntellectStat != 0)
            c.Intellect.AddModifier(new StatModifier(itemIntellectStat, StatModType.Flat, this));
        if (itemDefenseStat != 0)
            c.Defense.AddModifier(new StatModifier(itemDefenseStat, StatModType.Flat, this));

        if (itemStrengthPercent != 0)
            c.Strength.AddModifier(new StatModifier(itemStrengthPercent, StatModType.PercentMult, this));
        if (itemBloodPercent != 0)
            c.Blood.AddModifier(new StatModifier(itemBloodPercent, StatModType.PercentMult, this));
        if (itemMovementPercent != 0)
            c.Movement.AddModifier(new StatModifier(itemMovementPercent, StatModType.PercentMult, this));
        if (itemAttackSpeedPercent != 0)
            c.AttackSpeed.AddModifier(new StatModifier(itemAttackSpeedPercent, StatModType.PercentMult, this));
        if (itemIntellectPercent != 0)
            c.Intellect.AddModifier(new StatModifier(itemIntellectPercent, StatModType.PercentMult, this));
        if (itemDefensePercent != 0)
            c.Defense.AddModifier(new StatModifier(itemDefensePercent, StatModType.PercentMult, this));
    }

    public void Unequip(Character c)
    {
        c.Strength.RemoveAllModifiersFromSource(this);
        c.Intellect.RemoveAllModifiersFromSource(this);
        c.Movement.RemoveAllModifiersFromSource(this);
        c.Blood.RemoveAllModifiersFromSource(this);
        c.Defense.RemoveAllModifiersFromSource(this);
        c.AttackSpeed.RemoveAllModifiersFromSource(this);
    }
}
