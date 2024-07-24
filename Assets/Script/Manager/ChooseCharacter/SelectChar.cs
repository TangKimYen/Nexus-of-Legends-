using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectChar : MonoBehaviour
{
    public GameObject confirmationPopup;
    public TextMeshProUGUI confirmationText;

    private string characterId; // ID của nhân vật
    private string characterName; // Tên của nhân vật
    public CharSelectManager charSelectManager; // Gán trực tiếp qua Inspector

    void Start()
    {
        if (charSelectManager == null)
        {
            Debug.LogError("CharSelectManager not assigned.");
        }
    }

    public void SelectFireKnight()
    {
        characterId = "c03";
        characterName = "Fire Knight";
        ShowConfirmationPopup();
    }

    public void SelectLeafRanger()
    {
        characterId = "c02";
        characterName = "Leaf Ranger";
        ShowConfirmationPopup();
    }

    public void SelectWaterPriestess()
    {
        characterId = "c04";
        characterName = "Water Priestess";
        ShowConfirmationPopup();
    }

    public void SelectWindAssassin()
    {
        characterId = "c01";
        characterName = "Wind Assassin";
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
            // Gọi hàm SelectCharacter của CharSelectManager với 2 đối số
            charSelectManager.SelectCharacter(characterId, characterName);
            // Chuyển sang scene TitleScreen sau khi chọn nhân vật
            SceneManager.LoadScene("TitleScreen");
        }
        else
        {
            Debug.LogError("charSelectManager is null.");
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