using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using Firebase.Database;
using TMPro;
using Firebase.Extensions;
using System;
using Photon.Pun.Demo.PunBasics;

public class EnemiesController : MonoBehaviourPunCallbacks, IPunObservable
{
    public string monsterId;
    private GameObject player;
    public float speed = 3f;
    public bool chase = false;
    public Transform staringPoint;
    [SerializeField] private float distanceFrontOf;
    [SerializeField] private int pointBoss;

    private Animator anim;
    public int currentHealth;
    public int maxHealth;
    public int defenseEnemies;
    private bool isDead = false;
    private bool rewardGiven = false;

    [SerializeField] private int expReward;
    [SerializeField] private int goldReward;
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;
    private DatabaseReference databaseReference;
    [SerializeField] AudioSource deathSound;

    private enum MovementState { idle, running }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character");
        anim = GetComponent<Animator>();
        databaseReference = FirebaseDatabase.DefaultInstance.GetReference("monsters");
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
        UpdateHealthUI();
    }

    void LoadMaxHealthFromFirebase()
    {
        databaseReference.Child(monsterId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Unable to retrieve monster data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    try
                    {
                        maxHealth = int.Parse(snapshot.Child("health").Value.ToString());
                        defenseEnemies = int.Parse(snapshot.Child("defense").Value.ToString());
                        currentHealth = maxHealth; // Initialize currentHealth equal to maxHealth
                        Debug.Log("Max health loaded from Firebase: " + maxHealth);

                        // Uncomment the following lines to update health bar and text immediately
                        if (healthBar != null)
                        {
                            healthBar.fillAmount = 1;
                        }
                        UpdateHealthText();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error processing monster data: " + ex.Message);
                        SetDefaultHealth();
                    }
                }
                else
                {
                    Debug.LogWarning("Monster data does not exist in Firebase.");
                    SetDefaultHealth();
                }
            }
        });
    }

    void SetDefaultHealth()
    {
        maxHealth = 100; // Set default max health
        currentHealth = maxHealth;
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth + "/" + maxHealth;
        }
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
        if (isDead) return; // Check if the enemy is already dead
        isDead = true; // Set the flag to true
        rewardGiven = false; // Reset rewardGiven flag

        deathSound.Play();
        anim.SetTrigger("death");

        if (PhotonNetwork.IsMasterClient && !rewardGiven)
        {
            photonView.RPC("RPC_RewardExpAndGold", RpcTarget.All, expReward, goldReward);
        }
    }
    public void Hurt()
    {
        //hurtSoundEffect.Play();
        anim.SetTrigger("hurt");
    }

    [PunRPC]
    void RPC_RewardExpAndGold(int exp, int gold)
    {
        if (!rewardGiven)
        {
            rewardGiven = true;

            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Character");
            }

            if (player != null)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.AddExp(exp);
                    playerStats.AddGold(gold);
                }

                InGameManager.instance?.EnemyDefeated(gold, exp); // Notify GameManager
            }
            else
            {
                Debug.LogError("Player object is null.");
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
            if (PlayerStats.Instance != null)
            {
                int damageTaken = PlayerStats.Instance.CalculateDamage() - defenseEnemies;
                currentHealth -= damageTaken;
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
                UpdateHealthUI();
            }
            else
            {
                Debug.LogError("PlayerStats.Instance is null.");
            }
        }
    }

    [PunRPC]
    void RPC_UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthUI();
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
            UpdateHealthUI();
        }
    }
}
