using UnityEngine;

public abstract class EnemyStats : ScriptableObject
{
    [Header("Health")]
    public int maxHp = 3;

    [Header("Vision")]
    public float viewRange = 6f;
    public float fov = 120f;
    public float alertTime = 3f;
    public float noticeDelay = 1f;

    [Header("Attack")]
    public int damage = 1;
    [Range(0.3f, 0.8f)] public float windUp = 0.5f;
    public float cooldown = 0.6f;
}
