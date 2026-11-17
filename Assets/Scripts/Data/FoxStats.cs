using UnityEngine;

[CreateAssetMenu(fileName = "Fox_Stats", menuName = "Game/Fox Stats")]
public class FoxStats : EnemyStats
{
    [Header("Fox")]
    public float patrolSpeed = 1.7f;
    public float chaseSpeed = 4f;
    public float loseTime = 2f;
    public float attackRange = 3f;
}
