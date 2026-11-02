using UnityEngine;

public class Candy : Hazard
{
    protected override void OnHitPlayer(Health player)
    {
        player.Heal(1);
    }
}
