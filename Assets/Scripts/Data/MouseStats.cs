using UnityEngine;

[CreateAssetMenu(fileName = "Mouse_Stats", menuName = "Game/Mouse Stats")]
public class MouseStats : EnemyStats
{
    [Header("Mouse")]
    public float moveSpeed = 2f;
    public float preferredDistance = 5f;
    public float lookTime = 2f;
    public float giveUpTime = 3f;
    public float shotSpeed = 6f;
}
