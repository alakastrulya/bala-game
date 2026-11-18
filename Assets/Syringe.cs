using UnityEngine;

public class Syringe : Hazard
{
    const float LifeTime = 6f;

    [HideInInspector] public float speed;

    void Start()
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, LifeTime);
    }

    protected override void OnLanded()
    {
        Explode();
    }
}