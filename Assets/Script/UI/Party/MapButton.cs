using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    public string mapName;
    public string floor;
    public string levelRequire;
    public GameObject blockMap;
    public Sprite mapBG;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        int requiredLevel;
        if (int.TryParse(levelRequire, out requiredLevel))
        {
            int playerLevel = PlayerData.instance.level;
            if (playerLevel >= requiredLevel)
            {
                blockMap.SetActive(false);
                button.onClick.AddListener(OnMapButtonClicked);
            }
            else
            {
                // Disable the button if the player's level is too low
                button.interactable = false;
            }
        }
        else
        {
            Debug.LogError("Failed to parse level requirement.");
        }
    }

    void OnMapButtonClicked()
    {
        Debug.Log("Map selected: " + mapName);
        FindObjectOfType<ConnectToServer>().OnMapSelected(mapName, floor, levelRequire, mapBG);
    }
}
