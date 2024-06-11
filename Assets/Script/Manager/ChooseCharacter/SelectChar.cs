using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectChar : MonoBehaviour
{
    public GameObject confirmationPopup; // G�n GameObject c?a popup x�c nh?n v�o ?�y
    public TextMeshProUGUI confirmationText; // G�n th�nh ph?n Text c?a popup v�o ?�y

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
        // Logic ?? x? l� khi ng??i ch?i x�c nh?n ch?n nh�n v?t
        Debug.Log("Selected Character: " + selectedCharacter);
        confirmationPopup.SetActive(false);
    }

    public void CancelSelection()
    {
        confirmationPopup.SetActive(false);
    }
}

