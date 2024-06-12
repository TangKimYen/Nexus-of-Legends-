using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateIfNotMine : MonoBehaviourPunCallbacks
{
    Transform target;
    Vector3 velocity = Vector3.zero;

    [Range(0, 1)]
    public float smoothTime;
    public Vector3 positionOffSet;

    [Header("Axis Limitation")]
    public Vector2 xLimit;
    public Vector2 yLimit;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + positionOffSet;
        targetPosition = new Vector3(Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y), Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y), -10);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
