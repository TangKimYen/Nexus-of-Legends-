using UnityEngine;

public class ChatToggle : MonoBehaviour
{
    public GameObject chatPanel;
    private bool isActive = false;

    public void ToggleChatPanel()
    {
        if (isActive)
        {
            isActive = false;
            chatPanel.transform.localScale = new Vector3(5.22675f, 3.397555f, 4.1814f);
        }
        else
        {
            isActive = true;
            chatPanel.transform.localScale = Vector3.zero;
        }
    }

    public void ToggleSettingPanel()
    {
        if (isActive)
        {
            isActive = false;
            chatPanel.transform.localScale = new Vector3(8, 8, 8);
        }
        else
        {
            isActive = true;
            chatPanel.transform.localScale = Vector3.zero;
        }
    }
}
