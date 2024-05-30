using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisInPopupLog : MonoBehaviour
{
    public GameObject PopupLog;
    public GameObject PopupRegis;

    void Start()
    {
        // Ban ??u hi?n th? popup ??ng nh?p và ?n popup ??ng ký
        ShowLoginPopup();
    }

    // Hàm ?? hi?n th? popup ??ng nh?p và ?n popup ??ng ký
    public void ShowLoginPopup()
    {
        PopupLog.SetActive(true);
        PopupRegis.SetActive(false);
    }

    // Hàm ?? hi?n th? popup ??ng ký và ?n popup ??ng nh?p
    public void ShowRegisterPopup()
    {
        PopupLog.SetActive(false);
        PopupRegis.SetActive(true);
    }
}
