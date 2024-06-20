using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SkeletonController : MonoBehaviour
{
    private GameObject player;
    public float speed = 3f;
    public bool chase = false;
    public Transform staringPoint;
    [SerializeField] private float distanceFrontOf;

    [SerializeField] private int pointBoss;

    private Animator anim;
    private int countAttack = 0;
    [SerializeField] private int maxHealth;
    //[SerializeField] private AudioSource deathSoundEffect;
    //[SerializeField] private AudioSource hurtSoundEffect;

    private enum MovementState { idle, running }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character");
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Character");
        if (player == null)
        {
            return;
        }
        if (chase == true)
        {
            Chase();
        }
        else
        {
            ReturnStartPoint();
        }
        UpdateAnimationUpdate();
        Flip();
    }

    private void UpdateAnimationUpdate()
    {
        MovementState state;

        if (transform.position.x == staringPoint.transform.position.x)
        {
            state = MovementState.idle;
        }
        else if (transform.position.x == (player.transform.position.x + distanceFrontOf))
        {
            state = MovementState.idle;
        }
        else if (transform.position.x == (player.transform.position.x - distanceFrontOf))
        {
            state = MovementState.idle;
        }
        else
        {
            state = MovementState.running;
        }

        anim.SetInteger("state", (int)state);
    }

    private void ReturnStartPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, staringPoint.transform.position, speed * Time.deltaTime);
    }
    private void Chase()
    {
        Vector2 playerPosition = player.transform.position;
        if (transform.position.x > (player.transform.position.x + distanceFrontOf))
        {
            playerPosition.x += distanceFrontOf;
            playerPosition.y += distanceFrontOf;
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);

        }
        else if (transform.position.x < (player.transform.position.x + distanceFrontOf))
        {
            playerPosition.x -= distanceFrontOf;
            playerPosition.y += distanceFrontOf;
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);

        }
    }
    private void Flip()
    {
        if (transform.position.x > player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void Death()
    {
        //deathSoundEffect.Play();
        anim.SetTrigger("death");
    }
    public void Hurt()
    {
        //hurtSoundEffect.Play();
        anim.SetTrigger("hurt");
    }

    public void Deactive()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            maxHealth = maxHealth - 1;
            if (maxHealth > 0)
            {
                Hurt();
            }
            else
            {
                Death();
            }
        }
    }
}
