using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerListItem : MonoBehaviour
{
    public TMP_Text playerNameText;

    public void SetUp(PlayerData playerData)
    {
        playerNameText.text = playerData.NickName;
    }
}
