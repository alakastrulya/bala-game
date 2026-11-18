using UnityEngine;

[CreateAssetMenu(fileName = "Boss_Stats", menuName = "Game/Boss Stats")]
public class BossStats : ScriptableObject
{
    [Header("Health")]
    public int maxHp = 8;

    [Header("Syringe attack")]
    public float startDelay = 2f;
    public float warnTime = 0.8f;
    public float pause = 2.5f;
    public float pausePhase2 = 1.5f;
    public int countPhase1 = 3;
    public int countPhase2 = 5;
    public float anglePhase1 = 20f;
    public float anglePhase2 = 25f;
    public float syringeSpeed = 8f;

    [Header("Falling items")]
    public float spawnStartDelay = 1f;
    public float spawnWarnTime = 1f;
    public float spawnInterval = 2.5f;
    public float spawnIntervalPhase2 = 1.5f;
}
