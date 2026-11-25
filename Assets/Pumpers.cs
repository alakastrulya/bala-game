using UnityEngine;

public class Pumpers : Hazard
{
    protected override void OnLanded()
    {
        Invoke(nameof(Detonate), FindFirstObjectByType<Player2D>().Items.bombFuse);
    }

    void Detonate()
    {
        AudioManager.Play(Sfx.Explosion);
        Explode();
    }

    protected override void OnHitPlayer(Health player)
    {
        if (player != null) player.TakeDamage(player.GetComponent<Player2D>().Items.bombDamage);
    }
}