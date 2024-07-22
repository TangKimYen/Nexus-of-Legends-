using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Firebase.Database;
using Firebase;
using Firebase.Extensions;
using UnityEngine.TextCore.Text;
using Photon.Pun.Demo.PunBasics;
using System;
using Newtonsoft.Json;

[Serializable]
public class PlayerInformation
{
    public string characterId;
    public string emailInfo;
    public int exp;
    public float gem;
    public float gold;
    public string passwordHash;
    public string usernameInfo;
}

[System.Serializable]
public class PlayerData
{
    public string NickName;
    public int Level;
    public string CharacterId; 

    public PlayerData(string nickName, int level, string characterId)
    {
        NickName = nickName;
        Level = level;
        CharacterId = characterId;
    }
}
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
    public TextMeshProUGUI floorDisplay; 
    public TextMeshProUGUI levelRequireMapDisplay; 
    public TextMeshProUGUI currentMemberDisplay;
    public TextMeshProUGUI maxMemberDisplay;
    public Image mapBGDisplay;
    public GameObject listMapLayout;
    public GameObject partyOptionLayout;
    public GameObject invitationLayout;
    public GameObject inviteButton;
    public GameObject invitationPanel;
    public TextMeshProUGUI invitationText;

    public RoomInfo roomInfo;

    public int currentMember;

    public TextMeshProUGUI dungeonRoomDisplay;
    public TextMeshProUGUI levelRequireRoomDisplay;
    public Image mapBGRoomDisplay;

    public TextMeshProUGUI dungeonInvitationDisplay;
    public TextMeshProUGUI levelRequireInvitationDisplay;
    public Image mapBGInvitationDisplay;

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

    [SerializeField] Transform lobbyPlayerListContain;
    [SerializeField] GameObject lobbyPlayerItemPrefab;
    private List<PlayerData> lobbyPlayers = new List<PlayerData>();
    public PlayerData playerLobbyData;
    private DatabaseReference databaseReference;

    private string[] playerIds = new string[] { "thanhdat123", "nhuquynh", "Tlinh", "kimyen24" };
    private string playerId;
    public PlayerInformation playerInfo;

    private List<RoomInfo> availableRooms = new List<RoomInfo>();
    private Invitation currentInvitation; // Store the current invitation

    private void Awake()
    {
        Instance = this;
        // Initialize Firebase
        FirebaseApp app = FirebaseApp.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        GetLobbyPlayersFromFirebase();
    }

    private void Update()
    {
        currentMember = playerListContain.childCount;
        currentMemberDisplay.text = currentMember.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerId = playerIds[UnityEngine.Random.Range(0, playerIds.Length)];
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to server.");
            PhotonNetwork.ConnectUsingSettings();
        }
        LoadPlayerData();
        listMapLayout.SetActive(false);

        toggle1.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle2.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle3.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        toggle4.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });

        StartCoroutine(UpdateLobbyPlayerListCoroutine());
        ListenForInvitations();
        //TestDeserialization();
    }

    public void LoadPlayerData()
    {
        StartCoroutine(LoadPlayerDataEnum());
    }

    IEnumerator LoadPlayerDataEnum()
    {
        var serverData = databaseReference.Child("players").Child(playerId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        print("Process is Complete!");

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            print("Player data is found.");
            playerInfo = JsonUtility.FromJson<PlayerInformation>(jsonData);
        }
        else
        {
            print("Player data is not found.");
        }
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
        PhotonNetwork.NickName = playerInfo.usernameInfo;

        AddPlayerToFirebase(PhotonNetwork.LocalPlayer, playerInfo.exp, playerInfo.characterId);
        // Clear the current lobby players list
        lobbyPlayers.Clear();

        // Update the UI
        UpdateLobbyPlayerList();

        // Populate the lobby players list with the current players in the lobby
        GetLobbyPlayersFromFirebase();
    }

    private IEnumerator UpdateLobbyPlayerListCoroutine()
    {
        while (true)
        {
            UpdateLobbyPlayerList();
            GetLobbyPlayersFromFirebase();
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void AddPlayerToFirebase(Player player, int level, string characterId)
    {
        string playerKey = player.UserId ?? player.NickName;
        PlayerData playerData = new PlayerData(player.NickName, level, characterId);
        string json = JsonUtility.ToJson(playerData);
        databaseReference.Child("lobbyPlayers").Child(playerKey).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to add player to Firebase: " + task.Exception);
            }
        });
    }

    private void RemovePlayerFromFirebase(Player player)
    {
        string playerKey = player.UserId ?? player.NickName;
        databaseReference.Child("lobbyPlayers").Child(playerKey).RemoveValueAsync();
    }

    private void GetLobbyPlayersFromFirebase()
    {
        databaseReference.Child("lobbyPlayers").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve lobby players from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                lobbyPlayers.Clear();

                foreach (DataSnapshot playerSnapshot in snapshot.Children)
                {
                    string json = playerSnapshot.GetRawJsonValue();

                    if (!string.IsNullOrEmpty(json) && json.StartsWith("{"))
                    {
                        try
                        {
                            PlayerData newPlayerData = JsonUtility.FromJson<PlayerData>(json);
                            if (newPlayerData != null)
                            {
                                lobbyPlayers.Add(newPlayerData);
                            }
                            else
                            {
                                Debug.LogError("Failed to deserialize player data from JSON: " + json);
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError("Exception while deserializing player data: " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Invalid JSON string for player data snapshot: " + playerSnapshot.Key + ", JSON: " + json);
                    }
                }
                UpdateLobbyPlayerList();
            }
            else
            {
                Debug.LogWarning("Task to retrieve lobby players from Firebase is not completed.");
            }
        });
    }

    private void UpdateLobbyPlayerList()
    {
        // Clear the current UI list
        foreach (Transform child in lobbyPlayerListContain)
        {
            Destroy(child.gameObject);
        }

        // Populate the UI list with the updated lobby players list
        foreach (PlayerData playerData in lobbyPlayers)
        {
            GameObject playerItem = Instantiate(lobbyPlayerItemPrefab, lobbyPlayerListContain);
            RectTransform rectTransform = playerItem.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            LobbyPlayerListItem listItem = playerItem.GetComponent<LobbyPlayerListItem>();
            if (listItem != null)
            {
                listItem.SetUp(playerData);
            }
            else
            {
                Debug.LogError("Lobby Player List Item component is missing on the prefab.");
            }
        }
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

    public void OnInviteButtonClicked()
    {
        StartCoroutine(UpdateLobbyPlayerListCoroutine());
        invitationLayout.SetActive(true);
    }

    public void OnMapSelected(string map, string floor, string levelRequire, Sprite mapBG)
    {
        selectedMap = map;
        selectedFloor = floor;
        selectedLevelRequire = levelRequire;
        selectedMapBGName = mapBG.name;
        dungeonDisplay.text = floor; 
        levelRequireDisplay.text = levelRequire;
        mapBGDisplay.sprite = mapBG;
        floorDisplay.text = selectedFloor;
        levelRequireMapDisplay.text = selectedLevelRequire; 
        currentMemberDisplay.text = currentMember.ToString();
        maxMemberDisplay.text = maxPlayers.ToString();
        photonView.RPC("SetSelectedMap", RpcTarget.AllBuffered, selectedMap, selectedFloor, selectedLevelRequire, selectedMapBGName);
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

            // Set Custom Room Properties
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
            customRoomProperties["SelectedMap"] = selectedMap;
            customRoomProperties["SelectedFloor"] = selectedFloor;
            customRoomProperties["SelectedLevelRequire"] = selectedLevelRequire;
            customRoomProperties["SelectedMapBGName"] = selectedMapBGName;
            roomOptions.CustomRoomProperties = customRoomProperties;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "SelectedMap", "SelectedFloor", "SelectedLevelRequire", "SelectedMapBGName" };

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
        RemovePlayerFromFirebase(PhotonNetwork.LocalPlayer);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedMap"))
        {
            selectedMap = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedMap"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedFloor"))
        {
            selectedFloor = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedFloor"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedLevelRequire"))
        {
            selectedLevelRequire = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedLevelRequire"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedMapBGName"))
        {
            selectedMapBGName = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedMapBGName"];
        }

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
            UpdateRoomInfo();
            GameObject playerItem = PhotonNetwork.Instantiate(playerItemPrefab.name, Vector3.zero, Quaternion.identity);
            playerItem.transform.SetParent(playerListContain);
            RectTransform rectTransform = playerItem.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            playerItem.GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        inviteButton.SetActive(PhotonNetwork.IsMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        UpdateRoomInfo();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        dungeonRoomDisplay.text = selectedFloor;
        levelRequireRoomDisplay.text = selectedLevelRequire;
        mapBGRoomDisplay.sprite = Resources.Load<Sprite>(selectedMapBGName);
        Debug.Log("Floor: " + selectedFloor + ", Level Require: " + selectedLevelRequire + "MapBG: " + selectedMapBGName);
        currentMemberDisplay.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        maxMemberDisplay.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        GameObject playerItem = PhotonNetwork.Instantiate(playerItemPrefab.name, Vector3.zero, Quaternion.identity);
        playerItem.transform.SetParent(playerListContain);
        RectTransform rectTransform = playerItem.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = Vector3.zero;
        playerItem.GetComponent<PlayerListItem>().SetUp(newPlayer);
        UpdateRoomInfo();

        UpdateLobbyPlayerList();
        RemovePlayerFromFirebase(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        currentMemberDisplay.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        maxMemberDisplay.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        UpdateRoomInfo();

        UpdateLobbyPlayerList();
        AddPlayerToFirebase(otherPlayer, playerInfo.exp, playerInfo.characterId);
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

    private void UpdateRoomInfo()
    {
        currentMember = PhotonNetwork.CurrentRoom.PlayerCount;
        currentMemberDisplay.text = currentMember.ToString();
        maxMemberDisplay.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        dungeonRoomDisplay.text = selectedFloor;
        levelRequireRoomDisplay.text = selectedLevelRequire;
        mapBGRoomDisplay.sprite = Resources.Load<Sprite>(selectedMapBGName);

        roomNameText.text = selectedFloor;
        // Update Room Custom Properties
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedMap"))
        {
            selectedMap = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedMap"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedFloor"))
        {
            selectedFloor = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedFloor"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedLevelRequire"))
        {
            selectedLevelRequire = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedLevelRequire"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("SelectedMapBGName"))
        {
            selectedMapBGName = (string)PhotonNetwork.CurrentRoom.CustomProperties["SelectedMapBGName"];
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnect from server for reason " + cause.ToString());
        RemovePlayerFromFirebase(PhotonNetwork.LocalPlayer);
    }

    private void OnApplicationQuit()
    {
        RemovePlayerFromFirebase(PhotonNetwork.LocalPlayer);
    }

    public void StartGame()
    {
        RemovePlayerFromFirebase(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LoadLevel(selectedMap);
    }

    public void ReturnLobby()
    {
        RemovePlayerFromFirebase(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LoadLevel("MainLobby");
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
        availableRooms = roomList;

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

            // Get Info from Custom Room Properties
            string map = roomList[i].CustomProperties.ContainsKey("SelectedMap") ? (string)roomList[i].CustomProperties["SelectedMap"] : "Unknown";
            string floor = roomList[i].CustomProperties.ContainsKey("SelectedFloor") ? (string)roomList[i].CustomProperties["SelectedFloor"] : "Unknown";
            string levelRequire = roomList[i].CustomProperties.ContainsKey("SelectedLevelRequire") ? (string)roomList[i].CustomProperties["SelectedLevelRequire"] : "Unknown";
            string mapBGName = roomList[i].CustomProperties.ContainsKey("SelectedMapBGName") ? (string)roomList[i].CustomProperties["SelectedMapBGName"] : "Unknown";
            Sprite mapBG = Resources.Load<Sprite>(mapBGName);

            // Show info
            roomItem.SetRoomData(map, floor, levelRequire, mapBGName, roomList[i].PlayerCount, roomList[i].MaxPlayers);
        }
    }

    public void SendInvitation(string receiverId)
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Not connected to Photon.");
            return;
        }

        Invitation invitation = new Invitation(
            PhotonNetwork.LocalPlayer.NickName,
            receiverId,
            selectedFloor,
            selectedMap,
            selectedFloor,
            selectedLevelRequire,
            selectedMapBGName
        );

        string jsonInvitation = JsonUtility.ToJson(invitation);
        databaseReference.Child("invitations").Child(receiverId).SetRawJsonValueAsync(jsonInvitation).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to send invitation: " + task.Exception);
            }
            else
            {
                Debug.Log("Invitation sent to " + receiverId);
            }
        });
    }

    private void ListenForInvitations()
    {
        databaseReference.Child("invitations").Child(playerId).ValueChanged += HandleInvitationReceived;
    }

    private void HandleInvitationReceived(object sender, ValueChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogError("Error receiving invitation: " + e.DatabaseError.Message);
            return;
        }

        if (e.Snapshot.Value != null)
        {
            string jsonInvitation = e.Snapshot.GetRawJsonValue();
            Debug.Log("Received JSON: " + jsonInvitation);
            try
            {
                currentInvitation = JsonConvert.DeserializeObject<Invitation>(jsonInvitation);

                // Hiển thị lời mời tới người chơi (tuỳ chỉnh phần hiển thị UI này)
                Debug.Log("Invitation received from " + currentInvitation.SenderId);
                ShowInvitation(currentInvitation); // Gọi hàm để hiển thị lời mời
            }
            catch (ArgumentException ex)
            {
                Debug.LogError("ArgumentException while deserializing invitation: " + ex.Message);
                Debug.LogError("Stack Trace: " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception while deserializing invitation: " + ex.Message);
                Debug.LogError("Stack Trace: " + ex.StackTrace);
            }
        }
    }

    public void OnInviteButtonClicked(string receiverId)
    {
        SendInvitation(receiverId);
    }

    public void ShowInvitation(Invitation invitation)
    {
        invitationText.text = $"{invitation.SenderId} invite you to join {invitation.RoomName}.";
        invitationPanel.SetActive(true);

        dungeonInvitationDisplay.text = invitation.Floor;
        levelRequireInvitationDisplay.text = invitation.LevelRequirement;
        mapBGInvitationDisplay.sprite = Resources.Load<Sprite>(invitation.MapBackgroundName);

        currentInvitation = invitation;

        Button acceptButton = invitationPanel.transform.Find("AcceptButton").GetComponent<Button>();
        Button cancelButton = invitationPanel.transform.Find("CancelButton").GetComponent<Button>();

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(AcceptInvitation);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(DeclineInvitation);
    }

    public void AcceptInvitation()
    {
        Debug.Log($"Attempting to join room: {currentInvitation.RoomName}");
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Not connected to Photon.");
            return;
        }

        bool roomExists = availableRooms.Any(room => room.Name == currentInvitation.RoomName);

        if (!roomExists)
        {
            Debug.LogError($"Room '{currentInvitation.RoomName}' does not exist.");
            return;
        }

        PhotonNetwork.JoinRoom(currentInvitation.RoomName);
        invitationPanel.SetActive(false);
        DeleteInvitation(playerId);
    }

    public void DeclineInvitation()
    {
        invitationPanel.SetActive(false);
        DeleteInvitation(playerId);
    }

    private void DeleteInvitation(string playerId)
    {
        databaseReference.Child("invitations").Child(playerId).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to delete invitation: " + task.Exception);
            }
            else
            {
                Debug.Log("Invitation deleted successfully.");
            }
        });
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
    }
}
