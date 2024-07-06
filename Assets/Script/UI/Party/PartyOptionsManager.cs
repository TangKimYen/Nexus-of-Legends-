using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;

public class PartyOptionsManager : MonoBehaviourPunCallbacks
{
    public static PartyOptionsManager Instance;

    public TextMeshProUGUI dungeonDisplay;
    public TextMeshProUGUI levelRequireDisplay;
    public GameObject listMapLayout;
    public GameObject partyOptionLayout;

    [SerializeField] Transform playerListContain;
    [SerializeField] GameObject playerItemPrefab;
    [SerializeField] GameObject startGameButton;

    public ToggleGroup toggleGroup;
    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    public Toggle toggle4;

    public TextMeshProUGUI floorDisplay; // Thêm TextMeshPro ?? hi?n th? thông tin t?ng
    public TextMeshProUGUI levelRequireMapDisplay; // Thêm TextMeshPro ?? hi?n th? yêu c?u c?p ??
    public Image mapBGDisplay;

    private string selectedMap;
    private string selectedFloor;
    private string selectedLevelRequire;
    private Sprite selectedMapBG;
    private int maxPlayers = 4;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to server.");
            PhotonNetwork.ConnectUsingSettings();
        }

        listMapLayout.SetActive(false);

        toggle1.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle2.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle3.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle4.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server.");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.Instance.OpenMenu("Title");
        PhotonNetwork.NickName = "Nexus " + Random.Range(0, 1000).ToString("0000");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnect from server for reason " + cause.ToString());
    }

    public void OnSelectMapButtonClicked()
    {
        listMapLayout.SetActive(true);
    }

    public void OnMapSelected(string map, string floor, string levelRequire, Sprite mapBG)
    {
        selectedMap = map;
        selectedFloor = floor;
        selectedLevelRequire = levelRequire;
        selectedMapBG = mapBG;
        dungeonDisplay.text = floor; // C?p nh?t thông tin t?ng trên b?n ??
        levelRequireDisplay.text = levelRequire; // C?p nh?t yêu c?u c?p ?? trên b?n ??
        mapBGDisplay.sprite = mapBG;
        floorDisplay.text = selectedFloor; // C?p nh?t thông tin t?ng trên b?n ??
        levelRequireMapDisplay.text = selectedLevelRequire; // C?p nh?t yêu c?u c?p ?? trên b?n ??
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
            partyOptionLayout.SetActive(false);
            PhotonNetwork.CreateRoom(selectedFloor, roomOptions);
            MenuManager.Instance.OpenMenu("Loading");
        }
        else
        {
            Debug.LogError("Failed to create room. Please ensure all fields are filled correctly.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully");
        MenuManager.Instance.OpenMenu("Room");
        selectedFloor = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;
        photonView.RPC("SetSelectedMap", RpcTarget.AllBuffered, selectedMap, selectedFloor, selectedLevelRequire);
        foreach (Transform child in playerListContain)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerItemPrefab, playerListContain).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerItemPrefab, playerListContain).GetComponent<PlayerListItem>().SetUp(newPlayer);
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

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(selectedMap);
    }
}
