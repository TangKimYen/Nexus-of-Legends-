using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private GameObject[] hits;
    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;
    //[SerializeField] private AudioSource attackSoundEffect;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Attack();
        }
        else if (Input.GetButtonDown("Fire1") && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Skill1();
        }
        else if (Input.GetButtonDown("Fire2") && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Skill2();
        }
        else if (Input.GetButtonDown("Fire3") && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Skill3();
        }
        cooldownTimer += Time.deltaTime;
    }
    private void Attack()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("attack");
        cooldownTimer = 0;

    }

    private void ShootArrow()
    {
        GameObject arrow = hits[FindHit()];
        arrow.transform.position = hitPoint.position;
        arrow.GetComponent<ArrowHit>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private void Skill1()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("skill1");
        cooldownTimer = 0;
    }
    private void Skill2()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("skill2");
        cooldownTimer = 0;
    }
    private void Skill3()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("skill3");
        cooldownTimer = 0;
    }

    /*private void dashinskill3()
    {
        transform.position = new vector3(transform.position.x + 3.9f, transform.position.y, transform.position.z);
    }
    private void returninskill3()
    {
        transform.position = new vector3(transform.position.x - 3.9f, transform.position.y, transform.position.z);
    }*/

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
}