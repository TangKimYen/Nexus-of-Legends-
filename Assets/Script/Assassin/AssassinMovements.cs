using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AssassinMovements : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Animator anim;
    PhotonView view;
    private PhotonAnimatorView photonAnimatorView;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState { idle, running, jumping, falling }

    [SerializeField] private AudioSource jumpSoundEffect;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        photonAnimatorView = GetComponent<PhotonAnimatorView>();
    }

    // Update is called once per frame 
    private void Update()
    {
        if (view.IsMine)
        {
            dirX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

            if ((Input.GetButtonDown("Vertical") || Input.GetButtonDown("Jump")) && IsGrounded())
            {
                //jumpSoundEffect.Play();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            UpdateAnimationUpdate();
        }
    }

    private void UpdateAnimationUpdate()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            transform.localScale = Vector3.one;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
        // Ensure the PhotonAnimatorView component updates the parameter
        photonAnimatorView.SetParameterSynchronized("state", PhotonAnimatorView.ParameterType.Int, PhotonAnimatorView.SynchronizeType.Discrete);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    public bool canAttack()
    {
        return dirX == 0;
    }
}
