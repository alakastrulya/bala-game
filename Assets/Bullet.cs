using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 12f;

    void Start()
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Boss boss = other.GetComponentInParent<Boss>();
        if (boss != null)
        {
            boss.TakeHit(1);
            Destroy(gameObject);
            return;
        }

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeHit(1);
            Destroy(gameObject);
            return;
        }

        Hazard h = other.GetComponentInParent<Hazard>();
        if (h == null) return;

        h.HitByBullet();
        Destroy(gameObject);
    }
}