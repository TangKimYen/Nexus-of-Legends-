using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemId;
    public string itemName;
    public float itemCoin;
    public bool isGem;

    public Sprite icon;
}
