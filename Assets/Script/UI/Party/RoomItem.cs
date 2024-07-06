using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text levelRequiredText;
    [SerializeField] TMP_Text memberText;
    [SerializeField] Image mapBGSprite;

    public RoomInfo info;

    public void SetUp(RoomInfo _info)
    {
        info = _info;
        roomNameText.text = _info.Name;
    }

    public void SetRoomData(string map, string floor, string levelRequire, string mapBGName, int currentMember, int maxMembers)
    {
        levelRequiredText.text = $"Level Required: {levelRequire}";
        memberText.text = $"Members: {currentMember}/{maxMembers}";
        mapBGSprite.sprite = Resources.Load<Sprite>(mapBGName);
    }

    public void OnClick()
    {
        ConnectToServer.Instance.JoinRoom(info);
    }
}
