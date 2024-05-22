using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : MonoBehaviour
{
    public float speed;

    private bool isfacingRight = true;
    private Rigidbody2D rb;
    private Animator anim;
    private float direction;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        direction = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(direction*speed,rb.velocity.y);
        flip();
        anim.SetFloat("move", Mathf.Abs(direction));

    }

    void flip()
    {
        if(isfacingRight && direction < 0 || !isfacingRight && direction > 0) 
        {
            isfacingRight = !isfacingRight;
            Vector3 size = transform.localScale;
            size.x = size.x * -1;
            transform.localScale = size;
        }
    }
}
