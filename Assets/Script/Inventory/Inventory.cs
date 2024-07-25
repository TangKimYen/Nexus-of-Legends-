using UnityEngine;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField] public ItemDatabase itemDatabase;
    [SerializeField] public List<Item> items = new List<Item>();
    [SerializeField] private Transform itemsParent;
    [SerializeField] private ItemSlot[] itemSlots;
    private Dictionary<string, bool> itemActiveStates = new Dictionary<string, bool>();

    public string userName;
    private DataInventorySaver dataSaver;
    private DatabaseReference dbRef;

    public event Action<Item> OnItemRightClickedEvent;

    private void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dataSaver = GetComponent<DataInventorySaver>();
        LoadItemsFromFirebase();
        AddFirebaseListeners();
    }

    private void OnValidate()
    {
        if (itemsParent != null)
        {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
            Debug.Log($"ItemSlots count: {itemSlots.Length}");
        }
        else
        {
            Debug.LogWarning("ItemsParent is not assigned.");
        }

        RefreshUI();
    }

    public void ClearItems()
    {
        items.Clear();
        RefreshUI();
    }

    private void RefreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = items[i];
            itemSlots[i].OnRightClickEvent += HandleRightClick;
        }

        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = null;
            itemSlots[i].OnRightClickEvent -= HandleRightClick;
        }
    }

    public bool AddItem(Item item)
    {
        if (IsFull())
            return false;

        items.Add(item);
        RefreshUI();
        return true;
    }

    public bool RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            RefreshUI();
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        return items.Count >= itemSlots.Length;
    }

    public void LoadItemsFromFirebase()
    {
        StartCoroutine(LoadItemsFromFirebaseCoroutine());
    }

    private IEnumerator LoadItemsFromFirebaseCoroutine()
    {
        Debug.Log("Loading data from Firebase...");
        if (dbRef == null || string.IsNullOrEmpty(userName))
        {
            Debug.LogError("Database reference or username is null.");
            yield break;
        }

        var serverData = dbRef.Child("Inventory").Child(userName).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        if (serverData.Exception == null)
        {
            DataSnapshot snapshot = serverData.Result;
            Debug.Log("Data loaded from Firebase.");

            items.Clear();
            itemActiveStates.Clear();

            if (snapshot.HasChild("items"))
            {
                foreach (DataSnapshot itemSnapshot in snapshot.Child("items").Children)
                {
                    if (itemSnapshot.Child("itemId").Value == null || itemSnapshot.Child("isActive").Value == null)
                    {
                        Debug.LogWarning("Item snapshot has null value for itemId or isActive.");
                        continue; // Bỏ qua mục này nếu thiếu giá trị
                    }

                    string itemId = itemSnapshot.Child("itemId").Value.ToString();
                    bool isActive = (bool)itemSnapshot.Child("isActive").Value;

                    Item item = itemDatabase?.GetItemById(itemId);
                    if (item != null)
                    {
                        if (isActive)
                        {
                            AddItem(item);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Item with ID {itemId} not found in item database.");
                    }
                }
            }
            RefreshUI();
        }
        else
        {
            Debug.LogError("Error loading items from Firebase: " + serverData.Exception);
        }
    }






    private void AddFirebaseListeners()
    {
        dbRef.Child("Inventory").Child(userName).Child("items").ChildChanged += HandleItemChanged;
        dbRef.Child("Inventory").Child(userName).Child("items").ChildAdded += HandleItemAdded;
        dbRef.Child("Inventory").Child(userName).Child("items").ChildRemoved += HandleItemRemoved;
    }

    private void HandleItemChanged(object sender, ChildChangedEventArgs e)
    {
        string itemId = e.Snapshot.Child("itemId").Value.ToString();
        bool isActive = (bool)e.Snapshot.Child("isActive").Value;

        Item item = itemDatabase.GetItemById(itemId);
        if (item != null)
        {
            if (isActive && !items.Contains(item))
            {
                AddItem(item);
            }
            else if (!isActive && items.Contains(item))
            {
                RemoveItem(item);
            }
            RefreshUI();
        }
    }

    private void HandleItemAdded(object sender, ChildChangedEventArgs e)
    {
        if (e.Snapshot == null || itemDatabase == null) return;

        string itemId = e.Snapshot.Child("itemId").Value?.ToString();
        bool isActive = e.Snapshot.Child("isActive").Value != null && (bool)e.Snapshot.Child("isActive").Value;

        if (isActive)
        {
            Item item = itemDatabase.GetItemById(itemId);
            if (item != null)
            {
                AddItem(item);
                RefreshUI();
            }
        }
    }



    private void HandleItemRemoved(object sender, ChildChangedEventArgs e)
    {
        string itemId = e.Snapshot.Child("itemId").Value.ToString();

        Item item = itemDatabase.GetItemById(itemId);
        if (item != null)
        {
            RemoveItem(item);
            RefreshUI();
        }
    }

    private void HandleRightClick(Item item)
    {
        OnItemRightClickedEvent?.Invoke(item);
    }

    public void SaveItemsToFirebase()
    {
        Dictionary<string, object> updates = new Dictionary<string, object>();
        foreach (Item item in items)
        {
            if (itemActiveStates.TryGetValue(item.itemId, out bool isActive))
            {
                updates[$"items/{item.itemId}/isActive"] = isActive;
            }
        }

        dbRef.Child("Inventory").Child(userName).UpdateChildrenAsync(updates).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Items saved to Firebase successfully.");
            }
            else
            {
                Debug.LogError("Failed to save items to Firebase: " + task.Exception);
            }
        });
    }


    [Serializable]
    public class InventoryData
    {
        public List<string> itemIds;

        public InventoryData(List<string> itemIds)
        {
            this.itemIds = itemIds;
        }
    }

    void OnApplicationQuit()
    {
        SaveItemsToFirebase();
    }
}