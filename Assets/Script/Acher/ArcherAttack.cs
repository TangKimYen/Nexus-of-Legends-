using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Transform skill1Point;
    [SerializeField] private Transform skill2Point;
    [SerializeField] private Transform skill3Point;
    [SerializeField] private GameObject[] hits;
    [SerializeField] private float skill1Cooldown;
    [SerializeField] private GameObject[] skill1;
    [SerializeField] private float skill2Cooldown;
    [SerializeField] private GameObject[] skill2;
    [SerializeField] private float skill3Cooldown;
    [SerializeField] private GameObject[] skill3;
    private Animator anim;
    private float dirX = 0f;
    PhotonView view;

    private AssassinMovements playerMovement;
    private float cooldownAttack = Mathf.Infinity;
    private float cooldownSkill1 = Mathf.Infinity;
    private float cooldownSkill2 = Mathf.Infinity;
    private float cooldownSkill3 = Mathf.Infinity;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource skill1Sound;
    [SerializeField] private AudioSource skill2Sound;
    [SerializeField] private AudioSource skill3Sound;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<AssassinMovements>();
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (view.IsMine)
        {
            dirX = Input.GetAxisRaw("Horizontal");
            if (Input.GetMouseButton(0) && cooldownAttack > attackCooldown && playerMovement.canAttack())
            {
                attackSound.Play();
                Attack();
            }
            else if (Input.GetButtonDown("Fire1") && cooldownSkill1 > skill1Cooldown && playerMovement.canAttack())
            {
                skill1Sound.Play();
                Skill1();
            }
            else if (Input.GetButtonDown("Fire2") && cooldownSkill2 > skill2Cooldown && playerMovement.canAttack())
            {
                skill2Sound.Play();
                Skill2();
            }
            else if (Input.GetButtonDown("Fire3") && cooldownSkill3 > skill3Cooldown && playerMovement.canAttack())
            {
                skill3Sound.Play();
                Skill3();
            }
            cooldownAttack += Time.deltaTime;
            cooldownSkill1 += Time.deltaTime;
            cooldownSkill2 += Time.deltaTime;
            cooldownSkill3 += Time.deltaTime;
        }
    }


    private void ShootArrow()
    {
        GameObject arrow = hits[FindHit()];
        arrow.transform.position = hitPoint.position;
        arrow.GetComponent<ArrowHit>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
    private void Attack()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("attack");
        cooldownAttack = 0;
    }

    private void Skill1()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("skill1");
        skill1[FindSkill1()].transform.position = skill1Point.position;
        skill1[FindSkill1()].GetComponent<SkillHit>().SetDirection(Mathf.Sign(transform.localScale.x));
        cooldownSkill1 = 0;
    }
    private void Skill2()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("skill2");
        cooldownSkill2 = 0;
    }
    private void SpamSkill2()
    {
        skill2[FindSkill2()].transform.position = skill2Point.position;
        skill2[FindSkill2()].GetComponent<SkillHit>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
    private void Skill3()
    {
        //attackSoundEffect.Play();
        anim.SetTrigger("skill3");
        cooldownSkill3 = 0;
    }

    private void SpamSkill3()
    {
        skill3[FindSkill3()].transform.position = skill3Point.position;
        skill3[FindSkill3()].GetComponent<SkillHit>().SetDirection(Mathf.Sign(transform.localScale.x));
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
    private int FindSkill1()
    {
        for (int i = 0; i < skill1.Length; i++)
        {
            if (!skill1[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
    private int FindSkill2()
    {
        for (int i = 0; i < skill2.Length; i++)
        {
            if (!skill2[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
    private int FindSkill3()
    {
        for (int i = 0; i < skill3.Length; i++)
        {
            if (!skill3[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
}