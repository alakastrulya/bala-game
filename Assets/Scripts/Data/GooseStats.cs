using UnityEngine;

[CreateAssetMenu(fileName = "Goose_Stats", menuName = "Game/Goose Stats")]
public class GooseStats : EnemyStats
{
    [Header("Goose")]
    public float dashSpeed = 9f;
    public float dashTime = 0.8f;
}
