using UnityEngine;

public class ChatToggle : MonoBehaviour
{
    public GameObject chatPanel;

    public void ToggleChatPanel()
    {
        if (chatPanel != null)
        {
            chatPanel.SetActive(!chatPanel.activeSelf);
        }
    }
}
