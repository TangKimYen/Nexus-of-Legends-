using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemiesAttack : MonoBehaviourPunCallbacks
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float collDistance;
    [SerializeField] private int damage;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private GameObject[] hits;
    [SerializeField] private BoxCollider2D boxColl;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool twoTypeAttack;
    private float cooldownTimer = Mathf.Infinity;

    private Animator anim;
    private int randomAttack = 0;
    [SerializeField] AudioSource attackSound;
    //private PlayerLife playerLife;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        cooldownTimer += Time.deltaTime;

        //attack when the player is in sight
        if (PlayerInsight())
        {
            randomAttack = Random.Range(0, 2);
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                if (twoTypeAttack && randomAttack == 1)
                {
                    photonView.RPC("RPC_Attack", RpcTarget.All, "attack2");
                }
                else
                {
                    photonView.RPC("RPC_Attack", RpcTarget.All, "attack");
                }
            }
        }
    }

    [PunRPC]
    private void RPC_Attack(string attackType)
    {
        attackSound.Play();
        anim.SetTrigger(attackType);
    }

    private int FindHit()
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    private void Shoot()
    {
        cooldownTimer = 0;
        hits[FindHit()].transform.position = hitPoint.position;
        hits[FindHit()].GetComponent<RangeHits>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private bool PlayerInsight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxColl.bounds.center + transform.right * range * transform.localScale.x * collDistance,
            new Vector3(boxColl.bounds.size.x * range, boxColl.bounds.size.y, boxColl.bounds.size.z), 0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxColl.bounds.center + transform.right * range * transform.localScale.x * collDistance,
            new Vector3(boxColl.bounds.size.x * range, boxColl.bounds.size.y, boxColl.bounds.size.z));
    }
}
