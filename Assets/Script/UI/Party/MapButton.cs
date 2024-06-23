using UnityEngine;

public class MapButton : MonoBehaviour
{
    public string mapName; // T�n c?a b?n ??
    public string floor; // T?ng c?a b?n ??
    public string levelRequire; // Y�u c?u c?p ?? c?a b?n ??

    public void OnMapButtonClicked()
    {
        FindObjectOfType<MapManager>().OnMapSelected(mapName, floor, levelRequire);
    }
}
