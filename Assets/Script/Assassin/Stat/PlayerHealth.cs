using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

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

    //[SerializeField] private AudioSource deathSoundEffect;
    //[SerializeField] private AudioSource restartSoundEffect;

    // Start is called before the first frame update
    private void Awake()
    {
        //restartSoundEffect.Play();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        UpdateHealthUI();
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
        anim.SetTrigger("hurt");
    }

    [PunRPC]
    private void DieRPC()
    {
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        // You might want to add additional code here for handling the death (e.g., respawn, game over)
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
}
