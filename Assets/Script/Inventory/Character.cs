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

            // Lấy dữ liệu inventory từ Firebase
            var inventoryData = dbRef.Child("Inventory").Child(userName).GetValueAsync();
            yield return new WaitUntil(predicate: () => inventoryData.IsCompleted);

            DataSnapshot inventorySnapshot = inventoryData.Result;
            string inventoryJson = inventorySnapshot.GetRawJsonValue();

            if (inventoryJson != null)
            {
                dataToSaveInventory inventoryFromDB = JsonUtility.FromJson<dataToSaveInventory>(inventoryJson);
                foreach (var itemData in inventoryFromDB.items)
                {
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

    public void Equip(EquippableItem item)
    {
        Debug.Log("Equip được gọi với item: " + item);
        if (inventory.RemoveItem(item))
        {
            Debug.Log("Item đã được gỡ khỏi inventory");
            EquippableItem previousItem;
            if (equipmentPanel.AddItem(item, out previousItem))
            {
                Debug.Log("Item đã được thêm vào equipment panel");
                if (previousItem != null)
                {
                    Debug.Log("Previous item: " + previousItem);
                    inventory.AddItem(previousItem);
                    previousItem.Unequip(this);
                    statPanel.UpdateStatValues();
                }
                item.Equip(this);
                statPanel.UpdateStatValues();
            }
            else
            {
                Debug.Log("Không thể thêm item vào equipment panel");
                inventory.AddItem(item);
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
            baseAttackSpeed = AttackSpeed.BaseValue,
            equippedItemIds = new List<string>()
        };

        foreach (var slot in equipmentPanel.equipmentSlots)
        {
            if (slot.Item != null)
            {
                stats.equippedItemIds.Add(slot.Item.itemId); // Lưu ID của item đã trang bị
            }
        }

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
        public List<string> equippedItemIds;
    }

    void OnApplicationQuit()
    {
        SaveCharacterData();
        inventory.SaveItemsToFirebase();
    }


}