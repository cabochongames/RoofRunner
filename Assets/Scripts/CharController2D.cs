﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharController2D : MonoBehaviour {

    public float speed = 5;
    public bool facingRight = true;
    public float jumpSpeed = 5f;
    private Rigidbody2D rbody;
    private Animator animator;
    bool isJumping = false;
    public float groundCheckDist = 0.005f;
    private bool  isGrounded = false;
    private float jumpButtonPressTime;
    public float maxJumpTime = 0.2f;
    public Transform[] Feet;
    public static bool isAlive = true;

    public static CharController2D Instance;

    void Start()
    {
        isAlive = true;
        Instance = this;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.respawn);
    }

    void Awake()
    {

        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {

        if (transform.position.y < -2)//
        {
            if (isAlive)
            {
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.lose);
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (isAlive)
        {
            if (transform.position.x < -3)
            {
                transform.position = new Vector3(-3, transform.position.y, transform.position.z);
            }

            isGrounded = IsOnGround();

            float horzMove = Input.GetAxisRaw("Horizontal");
            rbody.AddForce(new Vector2(horzMove * speed, 0) * Time.deltaTime);
            if (horzMove > 0 && !facingRight)
            {
                FlipImage();
            }
            else if (horzMove < 0 && facingRight)
            {
                FlipImage();
            }

            isJumping = Input.GetAxis("Jump") > 0.1f && (jumpButtonPressTime < maxJumpTime);

            if (isJumping)
            {
                if (isGrounded) SoundManager.Instance.PlayOneShot(SoundManager.Instance.jump);
                rbody.velocity = new Vector2(rbody.velocity.x, jumpSpeed);
            }

            if (isGrounded)
            {
                isJumping = false;
                jumpButtonPressTime = 0f;
            }
            else
            {
                jumpButtonPressTime += Time.deltaTime;
            }

            animator.SetFloat("Speed", Mathf.Abs(rbody.velocity.x) * Time.deltaTime);
            animator.SetBool("Jumping", isJumping || !isGrounded);

        }
    }

   

    void FlipImage()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public bool IsOnGround()
    {

        // Check if contacting the ground straight down
        bool groundCheck1 = Physics2D.Raycast(Feet[0].position,
                                -Vector2.up, groundCheckDist);

        // Check if contacting ground to the right
        bool groundCheck2 = Physics2D.Raycast(Feet[1].position,
            -Vector2.up, groundCheckDist);

        if (groundCheck1 || groundCheck2)
            return true;

        return false;

    }

    // If Manny falls off the screen destroy the game object
    void OnBecameInvisible()
    {

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
                
                Die();
        }

    }
    public static void Die()
    {
        isAlive = false;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.lose);
        Instance.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        RigidbodyConstraints2D rbc = new RigidbodyConstraints2D();
        Instance.gameObject.GetComponent<Rigidbody2D>().constraints = rbc;
        Instance.gameObject.GetComponent<Rigidbody2D>().AddTorque(1000f);
    }
}
