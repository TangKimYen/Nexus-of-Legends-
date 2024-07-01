using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBackgroundRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial rotation of the background
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // Lock the background rotation to the initial rotation
        transform.rotation = initialRotation;
    }
}
