using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float explodeTime = 0.4f;
    protected Rigidbody2D rb;


    protected Animator animator;
    protected bool hasLanded;
    protected bool isDestroyed;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (isDestroyed) return;

        if (c.gameObject.CompareTag("Player"))
        {
            OnHitPlayer(c.gameObject.GetComponent<Health>());
            Explode();
            return;
        }

        bool onGround = c.gameObject.CompareTag("Ground");
        bool onHazard = c.gameObject.GetComponent<Hazard>() != null;

        if (onGround || onHazard)
        {
            StopRolling();

            if (!hasLanded)
            {
                hasLanded = true;
                OnLanded();
            }
        }
    }
    protected virtual void OnLanded()
    {
        Destroy(gameObject, 5f);
    }

    protected virtual void OnHitPlayer(Health player)
    {
        if (player != null) player.TakeDamage(1);
    }

    public void HitByBullet()
    {
        Explode();
    }

    protected void Explode()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (rb != null) rb.simulated = false;

        if (animator != null)
        {
            animator.SetBool("Explode", true);
            Destroy(gameObject, explodeTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void StopRolling()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.angularVelocity = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX
                       | RigidbodyConstraints2D.FreezeRotation;
    }
}