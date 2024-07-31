using UnityEngine;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System;
using Firebase.Extensions;

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

        if (PlayerData.instance != null && !string.IsNullOrEmpty(PlayerData.instance.username))
        {
            string username = PlayerData.instance.username;
            userName = username;

            LoadItemsFromFirebase();
        }

        dataSaver = GetComponent<DataInventorySaver>();
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

            foreach (DataSnapshot itemSnapshot in snapshot.Children)
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
        EquipItem(item);
    }

    private void EquipItem(Item item)
    {
        StartCoroutine(UpdateItemStatusCoroutine(item));
    }

    private IEnumerator UpdateItemStatusCoroutine(Item item)
    {
        string itemPath = $"Inventory/{userName}/{item.itemId}";

        // Lấy dữ liệu hiện tại của item
        var ipath = dbRef.Child(itemPath).GetValueAsync();
        yield return new WaitUntil(() => ipath.IsCompleted);

        if (ipath.Exception == null)
        {
            DataSnapshot snapshot = ipath.Result;

            // Kiểm tra nếu item có tồn tại
            if (snapshot.Exists)
            {
                Debug.Log("Item exists, updating isActive.");

                var updates = new Dictionary<string, object>
            {
                { "isActive", false }
            };

                // Cập nhật dữ liệu
                dbRef.Child(itemPath).UpdateChildrenAsync(updates)
                    .ContinueWithOnMainThread(task => {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("Error updating value: " + task.Exception);
                        }
                        else
                        {
                            Debug.Log("Update succeeded.");
                            Debug.Log($"Updated path: {itemPath}, Updates: {JsonUtility.ToJson(updates)}");
                        }
                    });
            }
            else
            {
                Debug.LogWarning("Item not found.");
            }
        }
        else
        {
            Debug.LogError("Error retrieving item data: " + ipath.Exception);
        }
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

    public void SellItem(string itemId, float itemPrice)
    {
        Item itemToRemove = items.Find(item => item.itemId == itemId);
        if (itemToRemove != null)
        {
            RemoveItem(itemToRemove);

            // Cập nhật vàng của người chơi
            int goldEarned = Mathf.FloorToInt(itemPrice * 0.2f); // Sử dụng giá trị float và chuyển đổi thành int
            PlayerData.instance.gold += goldEarned;
            SavePlayerGoldToFirebase(PlayerData.instance.gold);

            // Xóa item khỏi Firebase
            dbRef.Child("Inventory").Child(userName).Child(itemId).RemoveValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log($"Item {itemId} đã được xóa khỏi Firebase.");
                    }
                    else
                    {
                        Debug.LogError($"Lỗi khi xóa item {itemId} khỏi Firebase: {task.Exception}");
                    }
                });

            RefreshUI();
        }
        else
        {
            Debug.LogError($"Không tìm thấy item với ID: {itemId} trong Inventory.");
        }
    }

    private void SavePlayerGoldToFirebase(float newGold)
    {
        dbRef.Child("players").Child(userName).Child("gold").SetValueAsync(newGold)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Số vàng đã được cập nhật trên Firebase.");
                }
                else
                {
                    Debug.LogError("Lỗi khi cập nhật số vàng trên Firebase: " + task.Exception);
                }
            });
    }

    void OnApplicationQuit()
    {
        SaveItemsToFirebase();
    }
}