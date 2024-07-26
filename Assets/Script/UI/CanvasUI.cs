using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        target = Camera.main.transform;
    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(transform.position + target.rotation * Vector3.forward,
                             target.rotation * Vector3.up);
        }
        else
        {
            Debug.Log("Not Found Main Camera");
        }
    }
}
