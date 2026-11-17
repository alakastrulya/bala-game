using UnityEngine;

public class Pumpers : Hazard
{
    public AudioClip boomClip;

    protected override void OnLanded()
    {
        Invoke(nameof(Detonate), FindFirstObjectByType<Player2D>().Items.bombFuse);
    }

    void Detonate()
    {
        if (boomClip != null) AudioSource.PlayClipAtPoint(boomClip, transform.position);
        Explode();
    }

    protected override void OnHitPlayer(Health player)
    {
        if (player != null) player.TakeDamage(player.GetComponent<Player2D>().Items.bombDamage);
    }
}