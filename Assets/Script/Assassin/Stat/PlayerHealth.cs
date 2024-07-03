using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    private Rigidbody2D rb;
    private Animator anim;
    //public Image[] hearts;
    //[SerializeField] private Sprite FullHeart;
    //[SerializeField] private Sprite EmptyHeart;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
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
        //health = PlayerPrefs.GetInt("health", 5);
        //Physics2D.IgnoreLayerCollision(7, 8, false);
    }
    private void Update()
    {
        //foreach (Image img in hearts)
        //{
        //    img.sprite = EmptyHeart;
        //}
        //for (int i = 0; i < health; i++)
        //{
        //    hearts[i].sprite = FullHeart;
        //}
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
                Die();
            }
            else
            {
                Hurt();
            }
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
    }

    public void Hurt()
    {
        anim.SetTrigger("hurt");
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

    public void Die()
    {
        //deathSoundEffect.Play();
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
        //new WaitForSeconds(2);
        //int score = PlayerPrefs.GetInt("score", 0);
        //int highscore = PlayerPrefs.GetInt("highscore", 0);
        //GameOver(score, highscore);
        //PlayerPrefs.SetInt("score", 0);
        //PlayerPrefs.SetInt("health", 5);
        //PlayerPrefs.SetInt("key", 0);
    }

    //public void GameOver(int score, int highscore)
    //{
    //    gameOver.GameOver(score, highscore);
    //}
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
