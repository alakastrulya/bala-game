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
    [SerializeField] protected float alertTime = 3f;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Collider2D col;
    protected Health playerHealth;
    protected int facing = 1;
    protected float stateTimer;
    protected bool seesPlayer;

    Color baseColor;
    float alertTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;
        baseColor = sr.color;

        if (player == null)
            player = FindFirstObjectByType<Player2D>().transform;

        playerHealth = player.GetComponent<Health>();
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        if (alertTimer > 0f) alertTimer -= Time.deltaTime;
        seesPlayer = CanSeePlayer() || alertTimer > 0f;
        Think();
    }

    protected abstract void Think();

    public void Alert()
    {
        alertTimer = alertTime;
        Face(PlayerDX());
    }

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
        Bounds b = col.bounds;
        Vector2 front = new Vector2(b.center.x + dir * (b.extents.x + 0.1f), b.min.y + 0.1f);
        bool ground = Physics2D.Raycast(front, Vector2.down, 0.5f, obstacleMask);
        bool wall = Physics2D.Raycast(b.center, new Vector2(dir, 0f), b.extents.x + 0.15f, obstacleMask);
        return ground && !wall;
    }

    void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;
        Bounds b = c.bounds;
        for (int dir = -1; dir <= 1; dir += 2)
        {
            Vector2 front = new Vector2(b.center.x + dir * (b.extents.x + 0.1f), b.min.y + 0.1f);
            bool ground = Physics2D.Raycast(front, Vector2.down, 0.5f, obstacleMask);
            Gizmos.color = ground ? Color.green : Color.red;
            Gizmos.DrawLine(front, front + Vector2.down * 0.5f);

            bool wall = Physics2D.Raycast(b.center, new Vector2(dir, 0f), b.extents.x + 0.15f, obstacleMask);
            Gizmos.color = wall ? Color.red : Color.green;
            Gizmos.DrawLine(b.center, (Vector2)b.center + new Vector2(dir * (b.extents.x + 0.15f), 0f));
        }
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