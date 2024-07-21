using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectChar : MonoBehaviour
{
    public GameObject confirmationPopup; // Gán GameObject c?a popup xác nh?n vào ?ây
    public TextMeshProUGUI confirmationText; // Gán thành ph?n Text c?a popup vào ?ây

    private string selectedCharacter;

    public void SelectFireKnight()
    {
        selectedCharacter = "Fire Knight";
        ShowConfirmationPopup();
    }

    public void SelectLeafRanger()
    {
        selectedCharacter = "Leaf Ranger";
        ShowConfirmationPopup();
    }

    public void SelectWaterPriestess()
    {
        selectedCharacter = "Water Priestess";
        ShowConfirmationPopup();
    }

    public void SelectWindAssassin()
    {
        selectedCharacter = "Wind Assassin";
        ShowConfirmationPopup();
    }

    private void ShowConfirmationPopup()
    {
        confirmationText.text = "Are you sure you want to select the " + selectedCharacter + " as your character?";
        confirmationPopup.SetActive(true);
    }

    public void ConfirmSelection()
    {
        // Logic ?? x? lý khi ng??i ch?i xác nh?n ch?n nhân v?t
        Debug.Log("Selected Character: " + selectedCharacter);
        confirmationPopup.SetActive(false);
    }

    public void CancelSelection()
    {
        confirmationPopup.SetActive(false);
    }
}

