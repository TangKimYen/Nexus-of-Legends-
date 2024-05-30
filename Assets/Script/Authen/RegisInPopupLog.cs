using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisInPopupLog : MonoBehaviour
{
    public GameObject PopupLog;
    public GameObject PopupRegis;

    void Start()
    {
        // Ban ??u hi?n th? popup ??ng nh?p v� ?n popup ??ng k�
        ShowLoginPopup();
    }

    // H�m ?? hi?n th? popup ??ng nh?p v� ?n popup ??ng k�
    public void ShowLoginPopup()
    {
        PopupLog.SetActive(true);
        PopupRegis.SetActive(false);
    }

    // H�m ?? hi?n th? popup ??ng k� v� ?n popup ??ng nh?p
    public void ShowRegisterPopup()
    {
        PopupLog.SetActive(false);
        PopupRegis.SetActive(true);
    }
}
