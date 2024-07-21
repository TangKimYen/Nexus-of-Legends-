using TMPro;
using UnityEngine;

public class SelectChar : MonoBehaviour
{
    public GameObject confirmationPopup;
    public TextMeshProUGUI confirmationText;

    private string characterId;
    private string characterName;
    private string characterAvatarPrefabName;
    public CharSelectManager charSelectManager;

    void Start()
    {
        if (charSelectManager == null)
        {
            charSelectManager = FindObjectOfType<CharSelectManager>();
            if (charSelectManager == null)
            {
                Debug.LogError("CharSelectManager not assigned and not found in the scene.");
            }
            else
            {
                Debug.Log("CharSelectManager found and assigned automatically.");
            }
        }
        confirmationPopup.SetActive(false);
    }

    public void SelectFireKnight()
    {
        characterId = "c01";
        characterName = "Fire Knight";
        characterAvatarPrefabName = "FireKnightPrefab"; // Tên prefab của Fire Knight
        ShowConfirmationPopup();
    }

    public void SelectLeafRanger()
    {
        characterId = "c02";
        characterName = "Leaf Ranger";
        characterAvatarPrefabName = "LeafRangerPrefab"; // Tên prefab của Leaf Ranger
        ShowConfirmationPopup();
    }

    public void SelectWaterPriestess()
    {
        characterId = "c03";
        characterName = "Water Priestess";
        characterAvatarPrefabName = "WaterPriestessPrefab"; // Tên prefab của Water Priestess
        ShowConfirmationPopup();
    }

    public void SelectWindAssassin()
    {
        characterId = "c04";
        characterName = "Wind Assassin";
        characterAvatarPrefabName = "WindAssassinPrefab"; // Tên prefab của Wind Assassin
        ShowConfirmationPopup();
    }

    private void ShowConfirmationPopup()
    {
        if (confirmationText != null)
        {
            confirmationText.text = "Are you sure you want to select " + characterName + " as your character?";
            if (confirmationPopup != null)
            {
                confirmationPopup.SetActive(true);
                Debug.Log("Showing confirmation popup for " + characterName);
            }
            else
            {
                Debug.LogError("Confirmation popup is not assigned.");
            }
        }
        else
        {
            Debug.LogError("Confirmation text is not assigned.");
        }
    }

    public void ConfirmSelection()
    {
        Debug.Log("Confirmed selection of Character ID: " + characterId);
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(false);
        }
        else
        {
            Debug.LogError("Confirmation popup is not assigned.");
        }

        if (charSelectManager != null)
        {
            charSelectManager.SelectCharacter(characterId, characterName, characterAvatarPrefabName);
        }
        else
        {
            Debug.LogError("CharSelectManager is null.");
        }
    }

    public void CancelSelection()
    {
        if (confirmationPopup != null)
        {
            confirmationPopup.SetActive(false);
        }
        else
        {
            Debug.LogError("Confirmation popup is not assigned.");
        }
    }
}