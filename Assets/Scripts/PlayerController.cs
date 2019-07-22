﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    //Script References

    //Floats
    public float maxSpeed = 5;
    public float speed = 150f;
    public float jumpPower = 400f;

    //Booleans
    public bool grounded;
    public bool canDoubleJump;
    public bool hovering;
    public bool canMove;
    public bool onWall;

    //Player Stats
    public int currentHealth;
    public int maxHealth = 3;

    //References
    private Animator anim;
    private Rigidbody2D rb2d;

    public Vector3 respawnPosition;
    public LevelManager theLevelManager;

    //Sound Fx
    public AudioSource jumpSound;
    public AudioSource hurtSound;
    public AudioSource deathSound;

    //Special Bools ***********
    public int hasGun = 0;
    public bool facingRight;
    public bool facingLeft;

    //GameObject tv;
    //GameObject plasma;
    //int characterSelect;

    public Vector3 pos;

    private LastPosition lastPositionScript;
    private PlayerController thePlayerController;
    private Evolve evolveScript;
    private PlayerGun playerGun;
    private LevelEnd levelEndScript;

    void Start()
    {
        //Defaults to facing right.
        facingRight = true;

        //Enables mobility.
        canMove = true;

        //SpawnPoint
        respawnPosition = transform.position;

        //References to other scripts.
        theLevelManager = FindObjectOfType<LevelManager>();
        lastPositionScript = FindObjectOfType<LastPosition>();
        evolveScript = FindObjectOfType<Evolve>();
        thePlayerController = FindObjectOfType<PlayerController>();
        playerGun = FindObjectOfType<PlayerGun>();


        //Assign variables
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();

        //Start with full health
        currentHealth = maxHealth;

        transform.position = lastPositionScript.pos;
    }

    // Update is called once per frame
    void Update()
    {
        pos = this.transform.position;

        anim.SetBool("Grounded", grounded);
        anim.SetFloat("Speed", Mathf.Abs(rb2d.velocity.x));


        //Move left & right
        if (Input.GetAxisRaw("Horizontal") < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingLeft = true;
            facingRight = false;
        }

        if (Input.GetAxisRaw("Horizontal") > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingLeft = false;
            facingRight = true;

        }

        //if (facingLeft)
        //{
        //    playerGun.shotPoint.transform.localScale = new Vector3(-1, 1, 1);
        //}



        //Jumping
        if (Input.GetButtonDown("Jump"))

        {
            if (grounded && !onWall)
            {
                rb2d.AddForce(Vector2.up * jumpPower);
                canDoubleJump = true;
                jumpSound.Play();
            }
            else
            {
                if (canDoubleJump)
                {
                    canDoubleJump = false;
                    rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                    rb2d.AddForce(Vector2.up * jumpPower / 1.25f);
                    jumpSound.Play();
                }
            }
        }
        //Checks for health and allows death.
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (currentHealth <= 0)
        {

            theLevelManager.Respawn();
            gameObject.GetComponent<Animation>().Play("Flash");

        }

    }


    void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 easeVelocity = rb2d.velocity;
            easeVelocity.y = rb2d.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x *= 0.25f;

            float h = Input.GetAxis("Horizontal");

            //Friction Easing the x of player
            if (grounded)
            {
                rb2d.velocity = easeVelocity;
            }

            //Move Player
            rb2d.AddForce(Vector2.right * speed * h);

            //Limit Speed of left/right movement
            if (rb2d.velocity.x > maxSpeed)
            {
                rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);

            }
            if (rb2d.velocity.x < -maxSpeed)
            {
                rb2d.velocity = new Vector2(-maxSpeed, rb2d.velocity.y);

            }
        }
    }

    //Receiving Damage
    public void HurtPlayer(int damageToTake)
    {
        currentHealth -= damageToTake;
        hurtSound.Play();
        GetComponent<Animation>().Play("Flash");
        GetComponent<Animator>().Play("Damaged");
    }

    public IEnumerator Knockback(float knockDur, float knockbackPwr, Vector3 knockbackDir)
    {
        canMove = false;
        float timer = 0;

        while (knockDur > timer)
        {
            timer += Time.deltaTime;

            rb2d.AddForce(new Vector3(knockbackDir.x * -22, knockbackDir.y * -17, transform.position.z));
        }
        canMove = true;

        yield return 0;

    }

    //Check player entering the kill plane trigger to kill
    void OnTriggerEnter2D(Collider2D other)
    {
        

        if (other.tag == "KillPlane")
        {
            //gameObject.SetActive(false);

            //transform.position = respawnPosition;

            theLevelManager.Respawn();
        }

        if (other.tag == "Checkpoint")
        {
            respawnPosition = other.transform.position;
        }

        if (other.gameObject.tag == "Wall")
        {
            onWall = true;


        }
        else
        {

        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Hoverboard")
        {
            rb2d.rotation = 0;
        }

        if (other.gameObject.tag == "Wall")
        {
            onWall = false;
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            transform.parent = null;

            rb2d.rotation = 0;
        }

    }

    void OnEnable()
    {

        transform.position = lastPositionScript.pos;

    }

}