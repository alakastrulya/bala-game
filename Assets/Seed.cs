using UnityEngine;

public class Seed : Hazard
{
    const float LifeTime = 6f;

    [HideInInspector] public float speed;
    [HideInInspector] public int damage;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, LifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed || !other.CompareTag("Player")) return;

        OnHitPlayer(other.GetComponent<Health>());
        Explode();
    }

    protected override void OnHitPlayer(Health player)
    {
        if (player != null) player.TakeDamage(damage);
    }
}
