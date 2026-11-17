using UnityEngine;

public class Seed : Hazard
{
    const float LifeTime = 6f;

    [HideInInspector] public float speed;
    [HideInInspector] public int damage;

    void Start()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, LifeTime);
    }

    protected override void OnHitPlayer(Health player)
    {
        if (player != null) player.TakeDamage(damage);
    }

    protected override void OnLanded()
    {
        Explode();
    }
}
