using UnityEngine;
using TMPro;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected Transform player;
    [SerializeField] protected float viewRange = 6f;
    [SerializeField] protected float fov = 120f;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] protected bool spriteFacesRight = true;
    [SerializeField] protected Color warnColor = Color.red;
    [SerializeField] protected TMP_Text stateLabel;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Health playerHealth;
    protected int facing = 1;
    protected float stateTimer;
    protected bool seesPlayer;

    Color baseColor;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        baseColor = sr.color;

        if (player == null)
            player = FindFirstObjectByType<Player2D>().transform;

        playerHealth = player.GetComponent<Health>();
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        seesPlayer = CanSeePlayer();
        Think();
    }

    protected abstract void Think();

    protected bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        if (dist > viewRange) return false;
        if (Vector2.Angle(new Vector2(facing, 0f), toPlayer) > fov / 2f) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, dist, obstacleMask);
        return hit.collider == null;
    }

    protected void EnterState(string stateName, int stateId, bool warning)
    {
        stateTimer = 0f;
        sr.color = warning ? warnColor : baseColor;
        if (stateLabel != null) stateLabel.text = stateName;
        if (anim != null) anim.SetInteger("State", stateId);
    }

    protected void Face(float dirX)
    {
        if (dirX > 0f) facing = 1;
        if (dirX < 0f) facing = -1;
        sr.flipX = spriteFacesRight ? facing < 0 : facing > 0;
    }

    protected void MoveX(float speed)
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }

    protected float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.position);
    }

    protected float PlayerDX()
    {
        return player.position.x - transform.position.x;
    }

    protected bool CanWalk(int dir)
    {
        Vector2 pos = transform.position;
        bool ground = Physics2D.Raycast(pos + new Vector2(dir * 0.6f, 0f), Vector2.down, 1.5f, obstacleMask);
        bool wall = Physics2D.Raycast(pos + new Vector2(0f, 0.3f), new Vector2(dir, 0f), 0.7f, obstacleMask);
        return ground && !wall;
    }

    protected bool IsOnScreen()
    {
        return sr.isVisible;
    }

    protected void PlaySound(AudioClip clip)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
