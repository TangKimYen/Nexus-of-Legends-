using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemId;
    public string itemName;
    public int itemCoin;

    public Sprite icon;
}
