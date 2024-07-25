using Firebase.Database;
using NOL.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    DatabaseReference dbRef;
    public CharacterStat Strength;
    public CharacterStat Intellect;
    public CharacterStat Defense;
    public CharacterStat Blood;
    public CharacterStat Movement;
    public CharacterStat AttackSpeed;

    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;
    [SerializeField] StatPanel statPanel;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadCharacterData();
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

}
