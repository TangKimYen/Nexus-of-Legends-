using UnityEngine;
using Photon.Pun;
using TMPro;

public class MapManager : MonoBehaviourPunCallbacks
{
    public GameObject listMapLayout; // ??i t??ng ch?a danh sách b?n ?? (ViewListMap)
    public TextMeshProUGUI selectedMapDisplay; // Text ?? hi?n th? thông tin b?n ?? ?ã ch?n
    public TextMeshProUGUI selectedFloorDisplay; // Text ?? hi?n th? thông tin t?ng ?ã ch?n
    public TextMeshProUGUI selectedLevelRequireDisplay; // Text ?? hi?n th? thông tin yêu c?u c?p ?? ?ã ch?n
    public TextMeshProUGUI dungeonDisplay; // Text ?? hi?n th? thông tin Dungeon ?ã ch?n
    public TextMeshProUGUI levelRequireDisplay; // Text ?? hi?n th? thông tin LevelRequire ?ã ch?n

    private string selectedMap; // Bi?n l?u tr? b?n ?? ?ã ch?n
    private string selectedFloor; // Bi?n l?u tr? t?ng ?ã ch?n
    private string selectedLevelRequire; // Bi?n l?u tr? yêu c?u c?p ?? ?ã ch?n

    // G?i hàm này khi nh?n nút ch?n b?n ??
    public void OnSelectMapButtonClicked()
    {
        listMapLayout.SetActive(true); // Hi?n th? danh sách b?n ??
    }

    // G?i hàm này khi ch?n m?t b?n ?? t? danh sách
    [PunRPC]
    public void OnMapSelected(string map, string floor, string levelRequire)
    {
        selectedMap = map; // L?u l?i b?n ?? ?ã ch?n
        selectedFloor = floor; // L?u l?i t?ng ?ã ch?n
        selectedLevelRequire = levelRequire; // L?u l?i yêu c?u c?p ?? ?ã ch?n
        listMapLayout.SetActive(false); // ?n danh sách b?n ??

        // C?p nh?t giao di?n
        selectedMapDisplay.text = "Selected Map: " + selectedMap;
        selectedFloorDisplay.text = "Floor: " + selectedFloor;
        selectedLevelRequireDisplay.text = "Level: " + selectedLevelRequire;

        // C?p nh?t thông tin Dungeon và LevelRequire bên d??i
        dungeonDisplay.text = "Dungeon: " + selectedFloor;
        levelRequireDisplay.text = "LevelRequire: " + selectedLevelRequire;
    }

    // G?i hàm này ?? ch?n b?n ?? và ??ng b? v?i các ng??i ch?i khác trong phòng
    public void SelectMap(string map, string floor, string levelRequire)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("OnMapSelected", RpcTarget.AllBuffered, map, floor, levelRequire);
        }
        else
        {
            Debug.LogWarning("RPCs can only be sent in rooms. Call of \"OnMapSelected\" gets executed locally, if at all.");
        }
    }
}
