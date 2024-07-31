using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform shopSlotsParent;

    private ShopSlot[] shopSlots;
    private DateTime nextRefreshTime;

    private void Start()
    {
        shopSlots = shopSlotsParent.GetComponentsInChildren<ShopSlot>();
        LoadNextRefreshTime();
        LoadShopItems();
        CheckAndRefreshShop();
    }

    private void Update()
    {
        CheckAndRefreshShop();
    }

    private void PopulateShopSlots()
    {
        List<Item> availableItems = new List<Item>(itemDatabase.items);

        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (availableItems.Count == 0)
            {
                shopSlots[i].ClearSlot();
                continue;
            }

            int randomIndex = UnityEngine.Random.Range(0, availableItems.Count);
            Item randomItem = availableItems[randomIndex];
            availableItems.RemoveAt(randomIndex);

            if (randomItem != null)
            {
                shopSlots[i].SetItem(randomItem);
            }
            else
            {
                shopSlots[i].ClearSlot();
            }
        }

        SaveShopItems();
    }

    private void LoadNextRefreshTime()
    {
        string savedTime = PlayerPrefs.GetString("NextRefreshTime", string.Empty);
        if (!string.IsNullOrEmpty(savedTime))
        {
            if (DateTime.TryParse(savedTime, out DateTime parsedTime))
            {
                nextRefreshTime = parsedTime;
            }
            else
            {
                Debug.LogWarning("Invalid saved time format. Setting next refresh time to 7PM today.");
                SetNextRefreshTime();
            }
        }
        else
        {
            SetNextRefreshTime();
        }
    }

    private void CheckAndRefreshShop()
    {
        if (DateTime.Now >= nextRefreshTime)
        {
            PopulateShopSlots();
            SetNextRefreshTime();
        }
    }

    private void SetNextRefreshTime()
    {
        DateTime now = DateTime.Now;
        DateTime today12AM = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

        if (now > today12AM)
        {
            nextRefreshTime = today12AM.AddDays(1); // 12PM ngày mai
        }
        else
        {
            nextRefreshTime = today12AM; // 12PM hôm nay
        }

        PlayerPrefs.SetString("NextRefreshTime", nextRefreshTime.ToString("o")); // Sử dụng định dạng round-trip cho DateTime
        PlayerPrefs.Save(); // Đảm bảo PlayerPrefs được lưu ngay lập tức
    }

    private void SaveShopItems()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i].Item != null)
            {
                PlayerPrefs.SetString($"ShopSlot_{i}", shopSlots[i].Item.itemId);
            }
            else
            {
                PlayerPrefs.DeleteKey($"ShopSlot_{i}");
            }
        }
        PlayerPrefs.Save();
    }

    private void LoadShopItems()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            string itemId = PlayerPrefs.GetString($"ShopSlot_{i}", string.Empty);
            if (!string.IsNullOrEmpty(itemId))
            {
                Item item = itemDatabase.GetItemById(itemId);
                if (item != null)
                {
                    shopSlots[i].SetItem(item);
                }
                else
                {
                    shopSlots[i].ClearSlot();
                }
            }
            else
            {
                shopSlots[i].ClearSlot();
            }
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("NextRefreshTime", nextRefreshTime.ToString("o")); // Sử dụng định dạng round-trip cho DateTime
        PlayerPrefs.Save(); // Đảm bảo PlayerPrefs được lưu ngay lập tức
    }
}
