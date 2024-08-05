using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Collections;
using TMPro;
using UnityEngine.Networking;

public class DataMapLoader : MonoBehaviour
{
    // UI Elements
    public TMP_Text TitleText;
    public TMP_Text DescriptionText;
    public TMP_Text DungeonText;
    public TMP_Text LevelRequireText;
    public Image MapImage; // Image component for the map image
    public GameObject ViewMapInfo; // Popup UI

    // Firebase Database Reference
    private DatabaseReference dbRef;

    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        ViewMapInfo.SetActive(false); // Initially hide the popup
    }

    // Method to show the map popup with information based on mapId
    public void ShowMapPopup(string mapId)
    {
        // Start loading the map data
        StartCoroutine(LoadMapData(mapId));
    }

    // Coroutine to load map data from Firebase
    IEnumerator LoadMapData(string mapId)
    {
        var serverData = dbRef.Child("map").Child(mapId).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverData.IsCompleted);

        if (serverData.Exception != null)
        {
            Debug.LogError("Failed to load map data: " + serverData.Exception);
            yield break;
        }

        DataSnapshot snapshot = serverData.Result;
        string jsonData = snapshot.GetRawJsonValue();

        if (jsonData != null)
        {
            InformationMapData informationMap = JsonUtility.FromJson<InformationMapData>(jsonData);
            UpdateUI(informationMap);
        }
        else
        {
            Debug.LogWarning("Map data not found for mapId: " + mapId);
        }
    }

    // Method to update the UI with the loaded map data
    void UpdateUI(InformationMapData map)
    {
        if (map != null)
        {
            TitleText.text = map.title;
            DescriptionText.text = map.description;
            DungeonText.text = "" + map.dungeon;
            LevelRequireText.text = "" + map.levelrequire;

            // Load and display the map image
            StartCoroutine(LoadMapImage(map.imageUrl)); // Assuming imageUrl is a field in InformationMapData

            ViewMapInfo.SetActive(true); // Show the popup
        }
    }

    // Coroutine to load and set the map image
    IEnumerator LoadMapImage(string imageUrl)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                MapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    // Method to hide the map popup
    public void HideMapPopup()
    {
        ViewMapInfo.SetActive(false);
    }
}


// Define the InformationMap class (if not already defined elsewhere)
[System.Serializable]
public class InformationMapData
{
    public string title;
    public string description;
    public string dungeon;
    public string levelrequire;
    public string imageUrl; // URL of the map image
}

