using UnityEngine;

public class Candy : Hazard
{
    protected override void OnHitPlayer(Health player)
    {
        player.Heal(player.GetComponent<Player2D>().Items.candyHeal);
    }
}
