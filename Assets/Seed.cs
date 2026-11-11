using UnityEngine;

public class Seed : Hazard
{
    public float speed = 6f;

    void Start()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, 6f);
    }

    protected override void OnLanded()
    {
        Explode();
    }
}
