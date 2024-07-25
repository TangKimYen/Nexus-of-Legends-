using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using Firebase.Database;
using TMPro;

public class EnemiesController : MonoBehaviourPunCallbacks, IPunObservable
{
    private GameObject player;
    public float speed = 3f;
    public bool chase = false;
    public Transform staringPoint;
    [SerializeField] private float distanceFrontOf;
    [SerializeField] private int pointBoss;

    private Animator anim;
    public int currentHealth;
    public int maxHealth;

    [SerializeField] private int expReward;

    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Canvas healthCanvas;
    private DataEnemiesSaver dataSaver;

    private enum MovementState { idle, running }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character");
        anim = GetComponent<Animator>();
        dataSaver = GetComponent<DataEnemiesSaver>();
        //SetEventCamera();
        LoadMaxHealthFromFirebase();
    }

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

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
        //UpdateHealthUI();
    }

    void LoadMaxHealthFromFirebase()
    {
        dataSaver.LoadMonsterData(); // Load data from Firebase
        StartCoroutine(WaitForDataLoad());
    }

    IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => dataSaver.monsterStats != null);

        if (dataSaver.monsterStats != null && dataSaver.monsterStats.health > 0)
        {
            maxHealth = (int)dataSaver.monsterStats.health;
            currentHealth = maxHealth; // Init currentHealth equal maxHealth
        }
        else
        {
            maxHealth = 100; // Set default max health
            currentHealth = maxHealth;
        }

        //if (healthBar != null)
        //{
        //    healthBar.fillAmount = 1;
        //}
        //UpdateHealthText();
    }

    //private void UpdateHealthUI()
    //{
    //    if (healthBar != null)
    //    {
    //        healthBar.fillAmount = (float)currentHealth / maxHealth;
    //    }
    //    UpdateHealthText();
    //}

    //private void UpdateHealthText()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = currentHealth + "/" + maxHealth;
    //    }
    //}

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
        if (transform.position.x > (player.transform.position.x + 0.5f))
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
        photonView.RPC("RPC_RewardExp", RpcTarget.All, expReward);
    }
    public void Hurt()
    {
        //hurtSoundEffect.Play();
        anim.SetTrigger("hurt");
    }

    [PunRPC]
    void RPC_RewardExp(int exp)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Character"))
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddExp(exp);
            }
        }
    }

    public void Deactive()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            currentHealth = currentHealth - 30;
            photonView.RPC("RPC_UpdateHealth", RpcTarget.OthersBuffered, currentHealth);
            if (currentHealth > 0)
            {
                Hurt();
                photonView.RPC("RPC_Hurt", RpcTarget.OthersBuffered);
            }
            else
            {
                Death();
                photonView.RPC("RPC_Death", RpcTarget.OthersBuffered);
            }
            //UpdateHealthUI();
        }
    }

    [PunRPC]
    void RPC_UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        //UpdateHealthUI();
    }

    [PunRPC]
    void RPC_Hurt()
    {
        Hurt();
    }

    [PunRPC]
    void RPC_Death()
    {
        Death();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to others
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(currentHealth);
            stream.SendNext(maxHealth);
        }
        else
        {
            // Receive data
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            currentHealth = (int)stream.ReceiveNext();
            maxHealth = (int)stream.ReceiveNext();
            //UpdateHealthUI();
        }
    }
}
