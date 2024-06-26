using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NOL.CharacterStats;

public class Character
{
    public CharacterStat Strength;
}

public class Item
{
    public string Name;

    public void Equip(Character character)
    {
        character.Strength.AddModifier(new StatModifier(10, StatModType.Flat, this));
        character.Strength.AddModifier(new StatModifier(0.1f, StatModType.PercentAdd, this));

        foreach (var mod in character.Strength.StatModifiers)
        {
            Item item = mod.Source as Item;
            Debug.Log(item.Name + ": " + mod.Value);
        }
    }

    public void Unequip(Character character)
    {
        character.Strength.RemoveAllModifiersFromSource(this);
    }
}
