using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPrefabtoButton : MonoBehaviour
{
    public Button myButton; // Nút Button của bạn
    public GameObject prefab; // Prefab mà bạn muốn thêm
    // Start is called before the first frame update
    void Start()
    {
        if (myButton != null)
        {
            // Thêm sự kiện OnClick cho Button
            myButton.onClick.AddListener(AddPrefabToButtonClick);
        }
    }
    void AddPrefabToButtonClick()
    {
        if (prefab != null)
        {
            Vector3 centerPosition = new Vector3(0, 0, 0);
            // Tạo một instance của Prefab tại vị trí của Button
            Instantiate(prefab, centerPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
