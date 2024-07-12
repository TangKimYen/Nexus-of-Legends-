using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] GameObject joinChatButton;
    [SerializeField] GameObject chatPanel;
    [SerializeField] TMP_InputField chatField;
    [SerializeField] TMP_Text chatDisplay;
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] string username;

    bool isConnected;
    ChatClient chatClient;
    string currentChat;

    void Start()
    {
        // Không c?n kh?i t?o chatClient ? ?ây n?u dùng nút join chat
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void UsernameOnValueChange(string valueIn)
    {
        username = valueIn;
    }

    public void ChatConnectOnClick()
    {
        username = usernameInputField.text;
        isConnected = true;
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(username));
        Debug.Log("Connecting as " + username);
    }

    public void SendMessageOnClick()
    {
        if (!string.IsNullOrEmpty(chatField.text))
        {
            chatClient.PublishMessage("LobbyChannel", chatField.text);
            chatField.text = "";
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
        joinChatButton.SetActive(false);
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
            chatDisplay.text += senders[i] + ": " + messages[i] + "\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        // Không c?n thi?t cho ch?c n?ng chat công c?ng
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log(user + " is " + status);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatPanel.SetActive(true);
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
