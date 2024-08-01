using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform shopSlotsParent;

    private ShopSlot[] shopSlots;
    private DateTime nextRefreshTime;
    private DatabaseReference shopDatabaseReference;

    private void Start()
    {
        shopSlots = shopSlotsParent.GetComponentsInChildren<ShopSlot>();
        shopDatabaseReference = FirebaseDatabase.DefaultInstance.GetReference("Shop");

        LoadNextRefreshTime();
    }

    private void LoadNextRefreshTime()
    {
        shopDatabaseReference.Child("NextRefreshTime").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string savedTime = snapshot.Value.ToString();
                    if (DateTime.TryParse(savedTime, out nextRefreshTime))
                    {
                        CheckAndRefreshShop();
                    }
                    else
                    {
                        Debug.LogWarning("Invalid saved time format on Firebase. Setting next refresh time to midnight.");
                        SetNextRefreshTime();
                        PopulateShopSlots();
                    }
                }
                else
                {
                    SetNextRefreshTime();
                    PopulateShopSlots();
                }
            }
            else
            {
                Debug.LogError("Failed to load next refresh time from Firebase: " + task.Exception);
            }
        });
    }

    private void CheckAndRefreshShop()
    {
        if (DateTime.Now >= nextRefreshTime)
        {
            PopulateShopSlots();
            SetNextRefreshTime();
        }
        else
        {
            LoadShopItems();
        }
    }

    private void SetNextRefreshTime()
    {
        DateTime now = DateTime.Now;
        nextRefreshTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1);

        shopDatabaseReference.Child("NextRefreshTime").SetValueAsync(nextRefreshTime.ToString("o"));
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
                shopDatabaseReference.Child($"Slot_{i}").SetValueAsync(randomItem.itemId);
            }
            else
            {
                shopSlots[i].ClearSlot();
            }
        }
    }

    private void LoadShopItems()
    {
        shopDatabaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                for (int i = 0; i < shopSlots.Length; i++)
                {
                    string itemId = snapshot.Child($"Slot_{i}").Value as string;
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
            else
            {
                Debug.LogError("Failed to load shop items from Firebase: " + task.Exception);
            }
        });
    }
}
