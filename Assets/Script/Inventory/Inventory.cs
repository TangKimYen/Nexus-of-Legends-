using UnityEngine;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [SerializeField] public ItemDatabase itemDatabase;
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private Transform itemsParent;
    [SerializeField] private ItemSlot[] itemSlots;

    public string userName; // ID của người dùng để lấy dữ liệu từ Firebase
    private DataInventorySaver dataSaver; // Thêm tham chiếu đến DataInventorySaver
    private DatabaseReference dbRef;

    public event System.Action<Item> OnItemRightClickedEvent;

    private void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        dataSaver = GetComponent<DataInventorySaver>();
        LoadItemsFromFirebase();
        AddFirebaseListeners(); // Thêm listener để lắng nghe thay đổi từ Firebase
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
        RefreshUI(); // Cập nhật giao diện người dùng sau khi xóa các item
    }

    private void RefreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = items[i];
            itemSlots[i].OnRightClickEvent += HandleRightClick; // Kết nối sự kiện nhấp chuột phải
        }

        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = null;
            itemSlots[i].OnRightClickEvent -= HandleRightClick; // Ngắt kết nối sự kiện nhấp chuột phải
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
        Debug.Log("Starting data load from Firebase...");
        var serverData = dbRef.Child("Inventory").Child(userName).GetValueAsync();
        yield return new WaitUntil(() => serverData.IsCompleted);

        if (serverData.Exception == null)
        {
            DataSnapshot snapshot = serverData.Result;
            Debug.Log("Data loaded from Firebase.");

            items.Clear(); // Xóa danh sách hiện tại

            // Kiểm tra nếu snapshot chứa dữ liệu itemIds
            if (snapshot.HasChild("itemIds"))
            {
                List<string> itemIds = new List<string>();
                foreach (DataSnapshot itemSnapshot in snapshot.Child("itemIds").Children)
                {
                    string itemId = itemSnapshot.Value.ToString();
                    itemIds.Add(itemId);
                }

                LoadItemsFromDatabase(itemIds); // Gọi hàm tại đây
            }
            else
            {
                Debug.LogWarning("No itemIds found in Firebase.");
            }
        }
        else
        {
            Debug.LogError("Error loading items from Firebase: " + serverData.Exception);
        }
    }

    private void LoadItemsFromDatabase(List<string> itemIds)
    {
        Debug.Log($"Loading items for IDs: {string.Join(", ", itemIds)}");
        foreach (string itemId in itemIds)
        {
            Item item = itemDatabase.GetItemById(itemId);
            if (item != null)
            {
                Debug.Log("Item found in database: " + item.itemName);
                AddItem(item);
            }
            else
            {
                Debug.LogWarning("Item ID " + itemId + " not found in ItemDatabase.");
            }
        }
    }

    private void AddFirebaseListeners()
    {
        dbRef.Child("Inventory").Child(userName).Child("itemIds").ChildAdded += HandleChildAdded;
        dbRef.Child("Inventory").Child(userName).Child("itemIds").ChildRemoved += HandleChildRemoved;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs e)
    {
        string itemId = e.Snapshot.Value.ToString();
        Item item = itemDatabase.GetItemById(itemId);
        if (item != null)
        {
            AddItem(item);
        }
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs e)
    {
        string itemId = e.Snapshot.Value.ToString();
        Item item = itemDatabase.GetItemById(itemId);
        if (item != null)
        {
            RemoveItem(item);
        }
    }

    private void HandleRightClick(Item item)
    {
        OnItemRightClickedEvent?.Invoke(item);
    }
}
