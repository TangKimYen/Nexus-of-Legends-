using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private Image healthBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBarUp;
    [SerializeField] private TMP_Text healthTextUp;
    public int health;
    public int maxHealth;
    [SerializeField] private Transform healPoint;
    [SerializeField] private GameObject[] HealEffect;

    [SerializeField] public AudioSource deathSound;
    [SerializeField] public AudioSource hurtSound;
    private DatabaseReference reference;

    // Start is called before the first frame update
    private void Awake()
    {
        //restartSoundEffect.Play();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        if (photonView.IsMine)
        {
            LoadPlayerData();
        }
        else
        {
            // Load data from Photon custom properties
            UpdateHealthUI();
        }
    }
    private void Update()
    {
        UpdateHealthUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hits") || collision.gameObject.CompareTag("Enemies"))
        {
            TakeDamage(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Heal"))
        {
            HealPlayer();
        }
    }

    public void TakeDamage(GameObject attacker)
    {
        DataEnemiesSaver monster = attacker.GetComponent<DataEnemiesSaver>();
        if (monster != null)
        {
            float damage = monster.CalculateMonsterDamage();
            health -= Mathf.RoundToInt(damage);
            Debug.Log("Player took " + damage + " damage from " + monster.monsterStats.monsterName);

            if (health <= 0)
            {
                photonView.RPC("DieRPC", RpcTarget.All);
                InGameManager.instance.PlayerDied();
            }
            else
            {
                photonView.RPC("HurtRPC", RpcTarget.All);
            }
            UpdateHealthUI();
        }
    }

    private void HealPlayer()
    {
        if (health < maxHealth)
        {
            int bloodHeal;
            if (HealController.Instance.GetHealAmount() + health > maxHealth)
            {
                bloodHeal = HealController.Instance.GetHealAmount() - (HealController.Instance.GetHealAmount() + health - maxHealth);
            }
            else
            {
                bloodHeal = HealController.Instance.GetHealAmount();
            }
            health += bloodHeal;
        }
        Heal();
        photonView.RPC("HealEffectRPC", RpcTarget.All, healPoint.position, transform.localScale.x);
        UpdateHealthUI();
    }

    [PunRPC]
    private void HurtRPC()
    {
        hurtSound.Play();
        anim.SetTrigger("hurt");
    }

    [PunRPC]
    private void DieRPC()
    {
        deathSound.Play();
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void UpdateHealthUI()
    {
        healthBar.fillAmount = (float)health / maxHealth;
        healthText.text = health + " / " + maxHealth;
        healthBarUp.fillAmount = (float)health / maxHealth;
        healthTextUp.text = health + " / " + maxHealth;
    }

    [PunRPC]
    private void HealEffectRPC(Vector3 healPosition, float direction)
    {
        int effectIndex = FindHealEffect();
        HealEffect[effectIndex].transform.position = healPosition;
        HealEffect[effectIndex].GetComponent<SkillHit>().SetDirection(Mathf.Sign(direction));
    }

    private int FindHealEffect()
    {
        for (int i = 0; i < HealEffect.Length; i++)
        {
            if (!HealEffect[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    public void Heal()
    {
        int effectIndex = FindHealEffect();
        HealEffect[effectIndex].SetActive(true);
        HealEffect[effectIndex].transform.position = healPoint.position;
        HealEffect[effectIndex].GetComponent<SkillHit>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void LoadPlayerData()
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference("PlayerCurrentStat").Child(PlayerData.instance.username);
        reference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Unable to retrieve player data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshott = task.Result;
                if (snapshott.Exists)
                {
                    try
                    {
                        health = int.Parse(snapshott.Child("currentBlood").Value.ToString());
                        maxHealth = int.Parse(snapshott.Child("currentBlood").Value.ToString());
                        photonView.RPC("UpdateHealthRPC", RpcTarget.AllBuffered, health, maxHealth);
                        UpdateHealthUI();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error processing player data: " + ex.Message);
                    }
                }
                else
                {
                    Debug.LogWarning("Player data does not exist in Firebase.");
                }
            }
        });
    }

    [PunRPC]
    private void UpdateHealthRPC(int newHealth, int newMaxHealth)
    {
        health = newHealth;
        maxHealth = newMaxHealth;
        UpdateHealthUI();
    }
}
