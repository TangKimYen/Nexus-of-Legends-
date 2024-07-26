using Firebase.Database;
using NOL.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    public string characterId;
    public string userName;
    DatabaseReference dbRef;
    public CharacterStat Strength;
    public CharacterStat Intellect;
    public CharacterStat Defense;
    public CharacterStat Blood;
    public CharacterStat Movement;
    public CharacterStat AttackSpeed;

    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipmentPanel equipmentPanel;
    public StatPanel statPanel; // Thay đổi mức độ truy cập thành public

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        if (PlayerData.instance != null && !string.IsNullOrEmpty(PlayerData.instance.username))
        {
            string username = PlayerData.instance.username;
            userName = username;
            characterId = PlayerData.instance.characterId;
            LoadCharacterData();
        }
    }

    public void LoadCharacterData()
    {
        StartCoroutine(LoadCharacterDataEnum());
    }

    IEnumerator LoadCharacterDataEnum()
    {
        var serverData = dbRef.Child("characters").Child(PlayerData.instance.characterId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Character data is found.");
            CharacterBaseStats characterBaseStats = JsonUtility.FromJson<CharacterBaseStats>(jsonData);
            SetCharacterStats(characterBaseStats);

            // Tải dữ liệu inventory từ Firebase
            var inventoryData = dbRef.Child("Inventory").Child(userName).GetValueAsync();
            yield return new WaitUntil(predicate: () => inventoryData.IsCompleted);

            DataSnapshot inventorySnapshot = inventoryData.Result;
            if (inventorySnapshot.Exists)
            {
                foreach (DataSnapshot itemSnapshot in inventorySnapshot.Children)
                {
                    string itemJson = itemSnapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(itemJson))
                    {
                        ItemData itemData = JsonUtility.FromJson<ItemData>(itemJson);
                        if (!itemData.isActive)
                        {
                            Item item = inventory.itemDatabase.GetItemById(itemData.itemId);
                            if (item != null && item is EquippableItem equippableItem)
                            {
                                // Thêm item vào equipment panel và trang bị
                                equipmentPanel.AddItem(equippableItem, out _);
                                equippableItem.Equip(this);
                            }
                        }
                    }
                }
            }
            else
            {
                print("Inventory data is not found.");
            }

            statPanel.UpdateStatValues();
        }
        else
        {
            print("Character data is not found.");
        }
    }

    private void SetCharacterStats(CharacterBaseStats stats)
    {
        Strength.BaseValue = stats.baseStrength;
        Intellect.BaseValue = stats.baseIntellect;
        Defense.BaseValue = stats.baseDefense;
        Blood.BaseValue = stats.baseBlood;
        Movement.BaseValue = stats.baseMovement;
        AttackSpeed.BaseValue = stats.baseAttackSpeed;

        statPanel.UpdateStatValues();
    }

    private void Awake()
    {
        statPanel.SetStat(Strength, Intellect, Defense, Blood, Movement, AttackSpeed);
        statPanel.UpdateStatValues();

        inventory.OnItemRightClickedEvent += EquipFromInventory;
        equipmentPanel.OnItemRightClickedEvent += UnequipFromEquipPanel;
    }

    private void EquipFromInventory(Item item)
    {
        Debug.Log("EquipFromInventory được gọi với item: " + item);
        if (item is EquippableItem)
        {
            Equip((EquippableItem)item);
        }
    }

    private void UnequipFromEquipPanel(Item item)
    {
        if (item is EquippableItem)
        {
            Unequip((EquippableItem)item);
        }
    }

    public void Equip(EquippableItem newItem)
    {
        Debug.Log("Equip được gọi với item: " + newItem);

        // Kiểm tra xem item mới có thể trang bị được không
        if (inventory.RemoveItem(newItem))
        {
            Debug.Log("Item đã được gỡ khỏi inventory");

            // Tìm item hiện tại đang trang bị cùng loại
            EquippableItem currentItem = null;
            foreach (var slot in equipmentPanel.equipmentSlots)
            {
                if (slot.EquippableType == newItem.equippableType && slot.Item != null)
                {
                    currentItem = (EquippableItem)slot.Item;
                    break;
                }
            }

            // Gỡ bỏ item hiện tại nếu có
            if (currentItem != null)
            {
                Debug.Log("Item hiện tại: " + currentItem);
                equipmentPanel.RemoveItem(currentItem);
                currentItem.Unequip(this);
                inventory.AddItem(currentItem);
                StartCoroutine(UpdateItemStatusCoroutine(currentItem, true)); // Cập nhật trạng thái là true
            }

            // Thêm item mới vào equipment panel và trang bị
            if (equipmentPanel.AddItem(newItem, out _))
            {
                Debug.Log("Item mới đã được thêm vào equipment panel");
                newItem.Equip(this);
                statPanel.UpdateStatValues();
                StartCoroutine(UpdateItemStatusCoroutine(newItem, false)); // Cập nhật trạng thái là false
            }
            else
            {
                Debug.Log("Không thể thêm item vào equipment panel");
                inventory.AddItem(newItem);
            }
        }
    }


    public void Unequip(EquippableItem item)
    {
        if (!inventory.IsFull() && equipmentPanel.RemoveItem(item))
        {
            item.Unequip(this);
            statPanel.UpdateStatValues();
            inventory.AddItem(item);
            StartCoroutine(UpdateItemStatusCoroutine(item, true));
        }
    }

    private IEnumerator UpdateItemStatusCoroutine(Item item, bool isActive)
    {
        string itemPath = $"Inventory/{userName}/{item.itemId}";

        Debug.Log($"Updating item status for: {item.itemId} at path: {itemPath}");

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
                { "isActive", isActive }
            };

                // Cập nhật dữ liệu
                var updateTask = dbRef.Child(itemPath).UpdateChildrenAsync(updates);
                yield return new WaitUntil(() => updateTask.IsCompleted);

                if (updateTask.Exception != null)
                {
                    Debug.LogError("Error updating value: " + updateTask.Exception);
                }
                else
                {
                    Debug.Log("Update succeeded.");
                    Debug.Log($"Updated path: {itemPath}, Updates: {JsonUtility.ToJson(updates)}");
                }
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


    public void SaveCharacterData()
    {
        CharacterBaseStats stats = new CharacterBaseStats()
        {
            baseStrength = Strength.BaseValue,
            baseIntellect = Intellect.BaseValue,
            baseDefense = Defense.BaseValue,
            baseBlood = Blood.BaseValue,
            baseMovement = Movement.BaseValue,
            baseAttackSpeed = AttackSpeed.BaseValue
        };

        string json = JsonUtility.ToJson(stats);
        dbRef.Child("characters").Child(characterId).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Character data saved to Firebase successfully.");
            }
            else
            {
                Debug.LogError("Failed to save character data to Firebase: " + task.Exception);
            }
        });
    }


    [System.Serializable]
    public class CharacterBaseStats
    {
        public float baseStrength;
        public float baseIntellect;
        public float baseDefense;
        public float baseBlood;
        public float baseMovement;
        public float baseAttackSpeed;
    }

    void OnApplicationQuit()
    {
        SaveCharacterData();
        inventory.SaveItemsToFirebase();
    }


}