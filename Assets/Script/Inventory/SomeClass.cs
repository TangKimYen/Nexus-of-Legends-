using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SomeClass : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Character character;

    private void Start()
    {
        // Thiết lập userId tại đây, ví dụ: từ một hệ thống đăng nhập
        inventory.userName = "yentk";
        inventory.LoadItemsFromFirebase();
    }
}
