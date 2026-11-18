using UnityEngine;

[CreateAssetMenu(fileName = "Player_Stats", menuName = "Game/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float speed = 7f;
    public float jumpForce = 12f;

    [Header("Health")]
    public int maxHp = 3;
    public float invincibleTime = 2f;

    [Header("Shooting")]
    public float shotCooldown = 0.3f;
    public float bulletSpeed = 12f;
    public int bulletDamage = 1;
}
