using System.Collections.Generic;
using UnityEngine;

public enum LevelGoal { ReachExit, CollectAllCoins, DefeatAllEnemies }

[System.Serializable]
public class EnemySpawn
{
    public GameObject prefab;
    public Vector2 position;
    public float patrolDistance = 3f;
}

[CreateAssetMenu(fileName = "Level_01", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName = "Level 1";
    [TextArea] public string description = "Reach the yurt";
    public LevelGoal goal = LevelGoal.ReachExit;
    public float timeLimit = 90f;
    public Vector2 playerStart;

    [Header("Score")]
    public int pointsPerCoin = 100;
    public int pointsPerSecond = 10;

    [Header("Enemies")]
    public List<EnemySpawn> enemies = new List<EnemySpawn>();

    [Header("Coins")]
    public GameObject coinPrefab;
    public List<Vector2> coins = new List<Vector2>();
}
