using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelectPopup : MonoBehaviour
{
    public GameObject popupAcpt;
    private string selectedCharacter;
    private CharSelectManager charSelectManager;

    void Start()
    {
        popupAcpt.SetActive(false); // ??m b?o popup ?n khi b?t ??u
        charSelectManager = FindObjectOfType<CharSelectManager>();
    }

    public void ShowPopup(string characterName)
    {
        selectedCharacter = characterName;
        popupAcpt.SetActive(true);
    }

    public void OnConfirm()
    {
        charSelectManager.SelectCharacter(selectedCharacter);
        popupAcpt.SetActive(false);
    }

    public void OnCancel()
    {
        popupAcpt.SetActive(false);
    }
}
