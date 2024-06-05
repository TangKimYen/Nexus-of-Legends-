using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    //public Image[] hearts;
    //public StartGame gameOver;
    //[SerializeField] private Sprite FullHeart;
    //[SerializeField] private Sprite EmptyHeart;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

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
        if (collision.gameObject.CompareTag("Hits"))
        {
            health--;
            Debug.Log("Hurt");
            if (health <= 0)
            {
                Die();
            }
            else
            {
                Hurt();
            }
        }
        else if (collision.gameObject.CompareTag("Enemies"))
        {
            health--;
            Debug.Log("Hurt");
            if (health <= 0)
            {
                Die();
            }
            else
            {
                Hurt();
            }
        }
        //else if (collision.transform.tag == "Heart")
        //{
        //    if (health < maxHealth)
        //    {
        //        collision.gameObject.SetActive(false);
        //        health++;
        //        PlayerPrefs.SetInt("health", health);
        //    }
        //}
    }

    public void Hurt()
    {
        anim.SetTrigger("hurt");
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
