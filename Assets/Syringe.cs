using UnityEngine;

public class Syringe : Hazard
{
    public float speed = 8f;

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