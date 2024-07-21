using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    public string mapName; // T�n c?a b?n ??
    public string floor; // T?ng c?a b?n ??
    public string levelRequire; // Y�u c?u c?p ?? c?a b?n ??
    public Sprite mapBG;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnMapButtonClicked);
    }

    void OnMapButtonClicked()
    {
        Debug.Log("Map selected: " + mapName);
        FindObjectOfType<ConnectToServer>().OnMapSelected(mapName, floor, levelRequire, mapBG);
    }
}
