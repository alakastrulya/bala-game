using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    const float LifeTime = 3f;
    int damage;

    public void Init(float speed, int damage)
    {
        this.damage = damage;
        var rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, LifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Boss boss = other.GetComponentInParent<Boss>();
        if (boss != null)
        {
            boss.TakeHit(damage);
            Destroy(gameObject);
            return;
        }

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeHit(damage);
            Destroy(gameObject);
            return;
        }

        Hazard h = other.GetComponentInParent<Hazard>();
        if (h == null) return;

        h.HitByBullet();
        Destroy(gameObject);
    }
}