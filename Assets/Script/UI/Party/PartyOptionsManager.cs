using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class PartyOptionsManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI dungeonDisplay;
    public TextMeshProUGUI levelRequireDisplay;
    public GameObject listMapLayout;

    public ToggleGroup toggleGroup;
    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    public Toggle toggle4;

    public TextMeshProUGUI floorDisplay; // Thêm TextMeshPro ?? hi?n th? thông tin t?ng
    public TextMeshProUGUI levelRequireMapDisplay; // Thêm TextMeshPro ?? hi?n th? yêu c?u c?p ??

    private string selectedMap;
    private string selectedFloor;
    private string selectedLevelRequire;
    private int maxPlayers;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        listMapLayout.SetActive(false);

        toggle1.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle2.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle3.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle4.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
    }

    public void OnSelectMapButtonClicked()
    {
        listMapLayout.SetActive(true);
    }

    public void OnMapSelected(string map, string floor, string levelRequire)
    {
        selectedMap = map;
        selectedFloor = floor;
        selectedLevelRequire = levelRequire;
        dungeonDisplay.text = floor; // C?p nh?t thông tin t?ng trên b?n ??
        levelRequireDisplay.text = levelRequire; // C?p nh?t yêu c?u c?p ?? trên b?n ??
        floorDisplay.text = "Floor " + selectedFloor; // C?p nh?t thông tin t?ng trên b?n ??
        levelRequireMapDisplay.text = "Level " + selectedLevelRequire; // C?p nh?t yêu c?u c?p ?? trên b?n ??
        listMapLayout.SetActive(false);
    }

    public void OnToggleValueChanged()
    {
        if (toggle1.isOn) maxPlayers = 1;
        else if (toggle2.isOn) maxPlayers = 2;
        else if (toggle3.isOn) maxPlayers = 3;
        else if (toggle4.isOn) maxPlayers = 4;

        Debug.Log("Max Players set to: " + maxPlayers);
    }

    public void OnCreateRoomButtonClicked()
    {
        Debug.Log("Max Players: " + maxPlayers);
        Debug.Log("Selected Map: " + selectedMap);

        if (maxPlayers > 0 && !string.IsNullOrEmpty(selectedMap))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)maxPlayers;

            Debug.Log("Creating room...");
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
        else
        {
            Debug.LogError("Failed to create room. Please ensure all fields are filled correctly.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully");
        photonView.RPC("SetSelectedMap", RpcTarget.AllBuffered, selectedMap, selectedFloor, selectedLevelRequire);
        PhotonNetwork.LoadLevel("Map2");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
    }

    [PunRPC]
    void SetSelectedMap(string map, string floor, string levelRequire)
    {
        selectedMap = map;
        selectedFloor = floor;
        selectedLevelRequire = levelRequire;
        Debug.Log("Map set: " + map + ", Floor: " + floor + ", Level Require: " + levelRequire);
    }
}
