using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] public List<Item> items = new List<Item>();
    // Đảm bảo danh sách không rỗng

    public Item GetItemById(string id)
    {
        foreach (var item in items)
        {
            if (item != null && item.itemId == id)
            {
                return item;
            }
        }
        Debug.LogWarning($"No item found for ID: {id}");
        return null;
    }
}
