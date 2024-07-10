using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public static ConnectToServer Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContain;
    [SerializeField] GameObject roomItemPrefab;
    [SerializeField] Transform playerListContain;
    [SerializeField] GameObject playerItemPrefab;
    [SerializeField] GameObject startGameButton;

    public TextMeshProUGUI dungeonDisplay;
    public TextMeshProUGUI levelRequireDisplay;
    public TextMeshProUGUI floorDisplay; // Thêm TextMeshPro ?? hi?n th? thông tin t?ng
    public TextMeshProUGUI levelRequireMapDisplay; // Thêm TextMeshPro ?? hi?n th? yêu c?u c?p ??
    public TextMeshProUGUI currentMemberDisplay;
    public TextMeshProUGUI maxMemberDisplay;
    public Image mapBGDisplay;
    public GameObject listMapLayout;
    public GameObject partyOptionLayout;

    public RoomInfo roomInfo;

    public int currentMember;

    public TextMeshProUGUI dungeonRoomDisplay;
    public TextMeshProUGUI levelRequireRoomDisplay;
    public Image mapBGRoomDisplay;

    public ToggleGroup toggleGroup;
    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    public Toggle toggle4;

    public string selectedMap;
    public string selectedFloor;
    public string selectedLevelRequire;
    public string selectedMapBGName;
    public int maxPlayers = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        currentMember = playerListContain.childCount;
        currentMemberDisplay.text = currentMember.ToString();
    }

    // Start is called before the first frame update
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

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");
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
        selectedMapBGName = mapBG.name;
        dungeonDisplay.text = floor; // C?p nh?t thông tin t?ng trên b?n ??
        levelRequireDisplay.text = levelRequire; // C?p nh?t yêu c?u c?p ?? trên b?n ??
        mapBGDisplay.sprite = mapBG;
        floorDisplay.text = selectedFloor; // C?p nh?t thông tin t?ng trên b?n ??
        levelRequireMapDisplay.text = selectedLevelRequire; // C?p nh?t yêu c?u c?p ?? trên b?n ??
        currentMemberDisplay.text = currentMember.ToString();
        maxMemberDisplay.text = maxPlayers.ToString();
        photonView.RPC("SetSelectedMap", RpcTarget.AllBuffered, selectedMap, selectedFloor, selectedLevelRequire, selectedMapBGName);
        listMapLayout.SetActive(false);

        SendUpdatedMapData();
    }

    public void OnToggleValueChanged()
    {
        if (toggle1.isOn) maxPlayers = 1;
        else if (toggle2.isOn) maxPlayers = 2;
        else if (toggle3.isOn) maxPlayers = 3;
        else if (toggle4.isOn) maxPlayers = 4;

        Debug.Log("Max Players set to: " + maxPlayers);
        SendUpdatedMapData();
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
            partyOptionLayout.SetActive(false);
            MenuManager.Instance.OpenMenu("Error");
            errorText.text = "Failed to create room. Please ensure all fields are filled correctly.";
            Debug.LogError("Failed to create room. Please ensure all fields are filled correctly.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully");
        roomNameText.text = selectedFloor;
        dungeonRoomDisplay.text = selectedFloor;
        levelRequireRoomDisplay.text = selectedLevelRequire; 
        mapBGRoomDisplay.sprite = Resources.Load<Sprite>(selectedMapBGName);
        MenuManager.Instance.OpenMenu("Room");
        selectedFloor = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;
        
        foreach (Transform child in playerListContain)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerItemPrefab, playerListContain).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        SendUpdatedMapData();
        UpdateRoomInfo();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        currentMemberDisplay.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        maxMemberDisplay.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();    
        Instantiate(playerItemPrefab, playerListContain).GetComponent<PlayerListItem>().SetUp(newPlayer);
        UpdateRoomInfo();
        SendUpdatedMapData();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
    }

    [PunRPC]
    void SetSelectedMap(string map, string floor, string levelRequire, string mapBGName)
    {
        selectedMap = map;
        selectedFloor = floor;
        selectedLevelRequire = levelRequire;
        selectedMapBGName = mapBGName;
        UpdateRoomInfo();
        Debug.Log("Map set: " + map + ", Floor: " + floor + ", Level Require: " + levelRequire + "MapBG: " + mapBGName);
    }

    private void SendUpdatedMapData()
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("SetSelectedMap", RpcTarget.AllBuffered, selectedMap, selectedFloor, selectedLevelRequire, selectedMapBGName);
        }
    }

    private void UpdateRoomInfo()
    {
        currentMember = PhotonNetwork.CurrentRoom.PlayerCount;
        currentMemberDisplay.text = currentMember.ToString();
        maxMemberDisplay.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        dungeonRoomDisplay.text = selectedFloor;
        levelRequireRoomDisplay.text = selectedLevelRequire;
        mapBGRoomDisplay.sprite = Resources.Load<Sprite>(selectedMapBGName);

        roomNameText.text = selectedFloor;
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(selectedMap);
    }

    public void JoinRoom(RoomInfo info)
    {
        if (info.PlayerCount >= info.MaxPlayers)
        {
            // Notify the player that the room is full
            errorText.text = "Room is full!";
            MenuManager.Instance.OpenMenu("Error");
            return;
        }

        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        currentMemberDisplay.text = currentMember.ToString();
        MenuManager.Instance.OpenMenu("Title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContain)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            GameObject roomItemObject = Instantiate(roomItemPrefab, roomListContain);
            RoomItem roomItem = roomItemObject.GetComponent<RoomItem>();
            roomItem.SetUp(roomList[i]);

            roomItem.SetRoomData(selectedMap, selectedFloor, selectedLevelRequire, selectedMapBGName, currentMember, maxPlayers);
        }
    }
}
