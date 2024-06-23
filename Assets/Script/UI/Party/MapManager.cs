using UnityEngine;
using UnityEngine.UI;
using TMPro; // S? d?ng TextMeshPro n?u b?n d�ng TMP cho Text

public class MapManager : MonoBehaviour
{
    public GameObject listMapLayout; // ??i t??ng ch?a danh s�ch b?n ?? (ViewListMap)
    public TextMeshProUGUI selectedMapDisplay; // Text ?? hi?n th? th�ng tin b?n ?? ?� ch?n
    public TextMeshProUGUI selectedFloorDisplay; // Text ?? hi?n th? th�ng tin t?ng ?� ch?n
    public TextMeshProUGUI selectedLevelRequireDisplay; // Text ?? hi?n th? th�ng tin y�u c?u c?p ?? ?� ch?n
    public TextMeshProUGUI dungeonDisplay; // Text ?? hi?n th? th�ng tin Dungeon ?� ch?n
    public TextMeshProUGUI levelRequireDisplay; // Text ?? hi?n th? th�ng tin LevelRequire ?� ch?n

    private string selectedMap; // Bi?n l?u tr? b?n ?? ?� ch?n
    private string selectedFloor; // Bi?n l?u tr? t?ng ?� ch?n
    private string selectedLevelRequire; // Bi?n l?u tr? y�u c?u c?p ?? ?� ch?n

    // G?i h�m n�y khi nh?n n�t ch?n b?n ??
    public void OnSelectMapButtonClicked()
    {
        listMapLayout.SetActive(true); // Hi?n th? danh s�ch b?n ??
    }

    // G?i h�m n�y khi ch?n m?t b?n ?? t? danh s�ch
    public void OnMapSelected(string map, string floor, string levelRequire)
    {
        selectedMap = map; // L?u l?i b?n ?? ?� ch?n
        selectedFloor = floor; // L?u l?i t?ng ?� ch?n
        selectedLevelRequire = levelRequire; // L?u l?i y�u c?u c?p ?? ?� ch?n
        listMapLayout.SetActive(false); // ?n danh s�ch b?n ??

        // C?p nh?t giao di?n
        selectedMapDisplay.text = "Selected Map: " + selectedMap;
        selectedFloorDisplay.text = "Floor: " + selectedFloor;
        selectedLevelRequireDisplay.text = "Level: " + selectedLevelRequire;

        // C?p nh?t th�ng tin Dungeon v� LevelRequire b�n d??i
        dungeonDisplay.text = "" + selectedFloor;
        levelRequireDisplay.text = "" + selectedLevelRequire;
    }
}
