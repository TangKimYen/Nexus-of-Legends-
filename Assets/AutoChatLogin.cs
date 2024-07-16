using System.Collections;
using UnityEngine;

public class AutoChatLogin : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CheckForUsername());
    }

    private IEnumerator CheckForUsername()
    {
        while (PhotonChatManager.Instance == null || !PlayerPrefs.HasKey("username"))
        {
            Debug.Log("Waiting for PhotonChatManager and username to be set in PlayerPrefs...");
            yield return new WaitForSeconds(1); // Ki?m tra l?i m?i giây
        }

        string username = PlayerPrefs.GetString("username");

        // K?t n?i t?i Photon Chat
        PhotonChatManager.Instance.InitializeChat(username);
    }
}
