using UnityEngine;

public class MapButton : MonoBehaviour
{
    public string mapName; // Tên c?a b?n ??
    public string floor; // T?ng c?a b?n ??
    public string levelRequire; // Yêu c?u c?p ?? c?a b?n ??

    public void OnMapButtonClicked()
    {
        FindObjectOfType<MapManager>().OnMapSelected(mapName, floor, levelRequire);
    }
}
