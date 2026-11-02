using UnityEngine;

public class Pumpers : Hazard
{
    public float fuse = 2f;
    public AudioClip boomClip;

    protected override void OnLanded()
    {
        Invoke(nameof(Detonate), fuse);
    }

    void Detonate()
    {
        if (boomClip != null) AudioSource.PlayClipAtPoint(boomClip, transform.position);
        Explode();
    }

    protected override void OnHitPlayer(Health player)
    {
        if (player != null) player.TakeDamage(2);
    }
}