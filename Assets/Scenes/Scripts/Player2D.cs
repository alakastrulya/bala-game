using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Player2D : MonoBehaviour
{
    public float speed = 7f;
    public float jumpForce = 12f;
    public float fallThresholdY = -10f;

    private Animator animator;
    private Rigidbody2D rb;
    private Shooter shooter;
    private bool onGround;

    private Vector2 move;

    private int coins;
    private int coinsTotal;

    public AudioClip coinClip;
    private AudioSource audioSource;
    private Health health;
    public TMP_Text coinText;
    public bool finishOnAllCoins = false;
    public float finishDelay = 1f;
    private bool levelFinished;
    public WinMenu winMenu;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        shooter = GetComponent<Shooter>();
        health = GetComponent<Health>();
    }

    void Start()
    {
        coinsTotal = GameObject.FindGameObjectsWithTag("Coin").Length;
        Debug.Log("Coins in this level: " + coinsTotal);
        UpdateCoinUI();
    }


    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if (!onGround) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        onGround = false;
        if (animator != null) animator.SetBool("isJumping", true);
    }

    public void OnFire()
    {
        if (shooter != null) shooter.Shoot();
    }


    void Update()
    {
        if (transform.position.y < fallThresholdY)
        {
            RestartLevel();
        }
    }

    public void onLanding()
    {
        if (animator != null) animator.SetBool("isJumping", false);
    }

    void FixedUpdate()
    {
        float x = move.x;
        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);

        bool isMoving = Mathf.Abs(x) > 0.01f;
        if (animator != null)
        {
            animator.SetBool("Move", isMoving);
        }

        if (x > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (x < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            if (audioSource != null && coinClip != null)
            {
                audioSource.PlayOneShot(coinClip);
            }

            Destroy(other.gameObject);
            coins++;
            Debug.Log("Coins: " + coins + " / " + coinsTotal);
            UpdateCoinUI();

            if (finishOnAllCoins && coinsTotal > 0 && coins >= coinsTotal && !levelFinished)
            {
                levelFinished = true;
                Invoke(nameof(CompleteLevelAndLoadNext), finishDelay);
            }
        }

        if (other.CompareTag("Exit"))
        {
            if (levelFinished) return;

            if (coins >= coinsTotal)
            {
                levelFinished = true;
                if (winMenu != null) winMenu.ShowWin();
                else CompleteLevelAndLoadNext();
            }
            else
            {
                Debug.Log("Collect all coins first! " + coins + " / " + coinsTotal);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            onLanding();
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (health != null) health.TakeDamage(1);
        }
    }

    void RestartLevel()
    {
        Debug.Log("Game Over / Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CompleteLevelAndLoadNext()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else SceneManager.LoadScene(0);
    }

    void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = coins + " / " + coinsTotal;
    }
}