using TMPro;
using UnityEngine;

public class SelectChar : MonoBehaviour
{
    public GameObject confirmationPopup;
    public TextMeshProUGUI confirmationText;
    private string selectedCharacterId;
    private string selectedCharacterName;
    private CharSelectManager charSelectManager;

    void Start()
    {
        confirmationPopup.SetActive(false); // Đảm bảo popup ẩn khi bắt đầu
        charSelectManager = FindObjectOfType<CharSelectManager>();
        if (charSelectManager == null)
        {
            Debug.LogError("CharSelectManager not found in the scene.");
        }
    }

    public void SelectFireKnight()
    {
        SelectCharacter("c01", "Fire Knight");
    }

    public void SelectLeafRanger()
    {
        SelectCharacter("c02", "Leaf Ranger");
    }

    public void SelectWaterPriestess()
    {
        SelectCharacter("c03", "Water Priestess");
    }

    public void SelectWindAssassin()
    {
        SelectCharacter("c04", "Wind Assassin");
    }

    private void SelectCharacter(string characterId, string characterName)
    {
        selectedCharacterId = characterId;
        selectedCharacterName = characterName;
        ShowConfirmationPopup();
    }

    private void ShowConfirmationPopup()
    {
        if (confirmationText != null)
        {
            confirmationText.text = "Are you sure you want to select " + selectedCharacterName + " as your character?";
            if (confirmationPopup != null)
            {
                confirmationPopup.SetActive(true);
                Debug.Log("Showing confirmation popup for " + selectedCharacterName);
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

    public void OnConfirm()
    {
        if (charSelectManager != null)
        {
            Debug.Log("Confirming selection: ID = " + selectedCharacterId + ", Name = " + selectedCharacterName);
            charSelectManager.SelectCharacter(selectedCharacterId, selectedCharacterName);
            confirmationPopup.SetActive(false);
        }
        else
        {
            Debug.LogError("charSelectManager is null.");
        }
    }

    public void OnCancel()
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