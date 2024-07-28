using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance;

    [SerializeField] GameObject chatPanel;
    [SerializeField] TMP_InputField chatInputField;
    [SerializeField] TMP_Text chatDisplay;
    [SerializeField] ScrollRect scrollRect;
    private string username;

    private ChatClient chatClient;
    private bool isConnected;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        // L?y tên ng??i dùng t? PlayerData
        username = PlayerData.instance.username;
        Debug.Log("Logged in as: " + username);  // Ki?m tra giá tr? username
        ConnectToChat();
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }

        if (isConnected && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    public void ConnectToChat()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(username));
        Debug.Log("Connecting as " + username);
    }

    public void SendMessage()
    {
        if (!string.IsNullOrEmpty(chatInputField.text))
        {
            chatClient.PublishMessage("LobbyChannel", chatInputField.text);
            chatInputField.text = "";
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat state changed: " + state.ToString());
    }

    public void OnConnected()
    {
        Debug.Log("Connected to chat.");
        chatPanel.SetActive(true);
        chatClient.Subscribe(new string[] { "LobbyChannel" });
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected from chat.");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            chatDisplay.text += $"<color=#312938>{senders[i]}</color>: {messages[i]}\n";
        }
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;  // Cu?n xu?ng cu?i
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // Handle private messages if needed
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log(user + " is " + status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to channel.");
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("Unsubscribed from channel.");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log(user + " has subscribed to " + channel);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log(user + " has unsubscribed from " + channel);
    }

    public void OnChatError(short code, string message)
    {
        Debug.LogError("Chat error " + code + ": " + message);
    }
}
