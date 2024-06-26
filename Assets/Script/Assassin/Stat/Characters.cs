using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NOL.CharacterStats;

public class Characters
{
    public CharacterStat Strength;
}

public class Items
{
    public string Name;

    public void Equip(Characters character)
    {
        character.Strength.AddModifier(new StatModifier(10, StatModType.Flat, this));
        character.Strength.AddModifier(new StatModifier(0.1f, StatModType.PercentAdd, this));

        foreach (var mod in character.Strength.StatModifiers)
        {
            Items item = mod.Source as Items;
            Debug.Log(item.Name + ": " + mod.Value);
        }
    }

    public void Unequip(Characters character)
    {
        character.Strength.RemoveAllModifiersFromSource(this);
    }
}
