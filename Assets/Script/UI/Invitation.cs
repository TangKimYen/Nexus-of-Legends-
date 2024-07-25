using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invitation : MonoBehaviour
{
    public string SenderId;
    public string ReceiverId;
    public string RoomName;
    public string Map;
    public string Floor;
    public string LevelRequirement;
    public string MapBackgroundName;

    public Invitation() { }

    public Invitation(string senderId, string receiverId, string roomName, string map, string floor, string levelRequirement, string mapBackgroundName)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        RoomName = roomName;
        Map = map;
        Floor = floor;
        LevelRequirement = levelRequirement;
        MapBackgroundName = mapBackgroundName;
    }
}

