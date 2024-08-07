using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchInventoryAndProfile : MonoBehaviour
{
    public Button inventoryButton;
    public Button profileButton;
    public Button inventoryCloseButton;
    public Button profileCloseButton;
    public GameObject inventoryLayout;
    public GameObject profileLayout;
    public GameObject inventoryHolder;
    public GameObject panelFaded;

    private Vector3 profileOriginalPosition;
    private Vector3 profileCloseButtonOriginalPosition;

    private void Start()
    {
        // Lưu vị trí ban đầu của profile layout và profile close button
        profileOriginalPosition = profileLayout.transform.position;
        profileCloseButtonOriginalPosition = profileCloseButton.transform.position;

        HideInventory();
        HideProfile();
    }

    public void OnClickInventoryButton()
    {
        inventoryHolder.SetActive(true);
        panelFaded.SetActive(true);
        ShowInventory();
        ShowProfile();
    }

    public void OnClickProfileButton()
    {
        inventoryHolder.SetActive(true);
        panelFaded.SetActive(true);
        HideInventory();
        ShowProfile();
        SetProfileToCenter();
    }

    public void OnClickInventoryCloseButton()
    {
        HideInventory();
        HideProfile();
        inventoryHolder.SetActive(false);
        panelFaded.SetActive(false);
    }

    public void OnClickProfileCloseButton()
    {
        HideProfile();
        ResetProfilePosition();
        inventoryHolder.SetActive(false);
        panelFaded.SetActive(false);
    }

    private void ShowInventory()
    {
        inventoryLayout.SetActive(true);
        inventoryCloseButton.gameObject.SetActive(true);
    }

    private void HideInventory()
    {
        inventoryLayout.SetActive(false);
        inventoryCloseButton.gameObject.SetActive(false);
    }

    private void ShowProfile()
    {
        profileLayout.SetActive(true);
        profileCloseButton.gameObject.SetActive(true);
    }

    private void HideProfile()
    {
        profileLayout.SetActive(false);
        profileCloseButton.gameObject.SetActive(false);
    }

    private void SetProfileToCenter()
    {
        profileLayout.transform.position = new Vector3(960, 540, 0);
        profileCloseButton.transform.position = new Vector3(1275, 965, 0);
    }

    private void ResetProfilePosition()
    {
        profileLayout.transform.position = profileOriginalPosition;
        profileCloseButton.transform.position = profileCloseButtonOriginalPosition;
    }
}
