using System;
using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem.XR.Haptics;



[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    //Public modifiable properties
    [Range(3, 10)]
    public float speed = 6.0f;
    [Range(0.01f, 0.2f)]
    public float groundCheckRadius = 0.02f;
    public Transform respawnPoint;

    //Private Components
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private GroundCheck groundCheck;
    private bool isDead = false;
    private Coroutine speedChange = null;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip coinSFX;
    [SerializeField] private AudioClip deathSFX;


    public void SpeedChange()
    {
        if (speedChange != null)
        {
            StopCoroutine(speedChange);
            speedChange = null;
            speed /= 2.0f;
        }

        speedChange = StartCoroutine(SpeedChangeCoroutine());
    }

    IEnumerator SpeedChangeCoroutine()
    {
        speed *= 2.0f;
        Debug.Log("Speed has been changed to: " + speed);

        yield return new WaitForSeconds(5.0f);

        speed /= 2.0f;
        Debug.Log("Speed has been changed to: " + speed);
    }



    private int lives = 3;
    public int Lives
    {
        get { return lives; }
        set
        {
            if (value < 0)
            {
                GameOver();
                return;
            }

            if (lives > value)
            {
                Respawn();
            }

            lives = value;
            UpdateLivesUI();
            Debug.Log("Lives have been set to: " + lives);
        }
    }


    public TextMeshProUGUI scoreText; // Assign in Inspector
    public TextMeshProUGUI livesText; // Assign in Inspector


    private int score = 0;

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Lives: " + lives;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Play death sound
        if (deathSFX != null)
            AudioManager.Instance.PlaySFX(deathSFX);

        Lives -= 1;

        if (Lives <= 0)
        {
            Debug.Log("Player died: Game Over");
            GameOver();
        }
        else
        {
            Debug.Log("Player died: Respawning...");
            Respawn();
        }
    }


    private void Respawn()
    {
        // Reset position to respawn point
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            Debug.LogWarning("RespawnPoint not assigned! Player won't move.");
        }

        // Reset velocity so player doesn't keep old momentum
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Reset any states
        isDead = false;

        // Optionally reset animations, inputs, etc.
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.Rebind(); // reset animator to default state
    }

    private void GameOver()
    {
        GameManager.Instance?.GameOver();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        groundCheck = new GroundCheck(LayerMask.GetMask("Ground"), GetComponent<Collider2D>(), rb, ref groundCheckRadius);

        lives = 3;
        score = 0;
        isDead = false;

        UpdateLivesUI();
        UpdateScoreUI();
    }


    // Update is called once per frame
    void Update()
    {
        if (isDead) return; // Disable all controls if dead

        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
        {
            rb.linearVelocity = Vector2.zero;
            anim.speed = 0f; // Pause animation
            return;
        }
        else
        {
            anim.speed = 1f; // Resume animation if unpaused
        }

        AnimatorClipInfo[] curPlayingClips = anim.GetCurrentAnimatorClipInfo(0);
        //Update our ground check
        groundCheck.CheckIsGrounded();

        //check for inputs
        float hInput = Input.GetAxis("Horizontal");

        if (curPlayingClips.Length > 0)
        {
            if (!(curPlayingClips[0].clip.name == "Fire"))
            {
                //apply physics and mechanics
                rb.linearVelocity = new Vector2(hInput * speed, rb.linearVelocity.y);

                if (Input.GetButtonDown("Fire1") && groundCheck.IsGrounded) anim.SetTrigger("Fire");
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }


        if (Input.GetButtonDown("Jump") && groundCheck.IsGrounded)
        {
            rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

            // Play jump sound
            if (jumpSFX != null)
                AudioManager.Instance.PlaySFX(jumpSFX);
        }


        //apply changes to look
        SpriteFlip(hInput);

        //apply animations
        anim.SetFloat("hInput", Mathf.Abs(hInput));
        anim.SetBool("isGrounded", groundCheck.IsGrounded);
    }

    void SpriteFlip(float hInput)
    {
        if (hInput != 0) sr.flipX = (hInput < 0);

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Goomba") ||
            other.CompareTag("PirahnaPlant"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
                    Destroy(other); // Or call enemy.Die()
                    AddScore(100);
                    return;
                }
            }

            Die();
        }
        else if (other.CompareTag("Koopa"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    // Hit Koopa from above: Koopa handles behavior
                    return;
                }
            }

            // Hit Koopa from side or bottom
            Koopa koopa = other.GetComponent<Koopa>();
            if (koopa != null && koopa.IsShell && koopa.IsSliding)
            {
                // Hit by sliding shell
                Die();
            }
            else
            {
                // Hit by walking Koopa from side or bottom
                Die();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup"))
        {
            if (coinSFX != null)
                AudioManager.Instance.PlaySFX(coinSFX);

            Destroy(collision.gameObject);
        }

        else if (collision.CompareTag("EnemyProjectile"))
        {
            Die();
            Destroy(collision.gameObject);
        }
    }
}