using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageLogin : MonoBehaviour
{
    public GameObject PopupLog;

    void Start()
    {
        // ?n popup ??ng nh?p khi b?t ??u
        if (PopupLog != null)
        {
            PopupLog.SetActive(true);
        }
    }

    // H�m ?? hi?n th? popup ??ng nh?p
    public void ShowLoginPopup()
    {
        if (PopupLog != null)
        {
            PopupLog.SetActive(true);
        }
    }
}
