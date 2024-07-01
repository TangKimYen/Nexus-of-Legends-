using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHit : MonoBehaviourPun
{
    [SerializeField] private float speed;
    [SerializeField] private float maxLifetime;
    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime)
        {
            gameObject.SetActive(false);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemies"))
        {
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");
        }
    }*/

    public void SetDirection(float _direction)
    {
        photonView.RPC("RPC_SetDirection", RpcTarget.All, _direction);
    }

    [PunRPC]
    public void RPC_SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void DashInSkill3()
    {
        if (transform.localScale.x > 0)
        {
            transform.position = new Vector3(transform.position.x + 3.9f, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x - 3.9f, transform.position.y, transform.position.z);
        }
    }
    private void ReturnInSkill3()
    {
        if (transform.localScale.x > 0)
        {
            transform.position = new Vector3(transform.position.x - 3.9f, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x + 3.9f, transform.position.y, transform.position.z);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
