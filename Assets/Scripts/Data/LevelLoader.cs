using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }

    [SerializeField] LevelData[] levels;
    [SerializeField] Player2D player;
    [SerializeField] WinMenu winMenu;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] float introTime = 2.5f;
    [SerializeField] int previewLevel;

    LevelData data;
    float timeLeft;
    bool finished;
    Coroutine intro;
    readonly List<GameObject> enemies = new List<GameObject>();

    public LevelData Data => data;
    public int Index => GameSession.LevelIndex;
    public float TimeLeft => timeLeft;
    public bool HasNextLevel => GameSession.LevelIndex + 1 < levels.Length;

    void Awake()
    {
        Instance = this;
        if (Application.isEditor && !GameSession.StartedFromMenu) GameSession.LevelIndex = previewLevel;
        GameSession.LevelIndex = Mathf.Clamp(GameSession.LevelIndex, 0, levels.Length - 1);
        data = levels[GameSession.LevelIndex];

        player.transform.position = data.playerStart;

        foreach (EnemySpawn e in data.enemies) SpawnEnemy(e);
        foreach (Vector2 c in data.coins) Instantiate(data.coinPrefab, c, Quaternion.identity);

        timeLeft = data.timeLimit;
        intro = StartCoroutine(ShowIntro());
    }

    IEnumerator ShowIntro()
    {
        if (levelText == null) yield break;
        levelText.text = data.levelName + "\n" + data.description;
        yield return new WaitForSeconds(introTime);
        levelText.text = data.levelName;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void SpawnEnemy(EnemySpawn e)
    {
        GameObject go = Instantiate(e.prefab, e.position, Quaternion.identity);
        enemies.Add(go);

        FoxPatrol fox = go.GetComponent<FoxPatrol>();
        if (fox != null) fox.SetPatrol(e.position.x - e.patrolDistance, e.position.x + e.patrolDistance);
    }

    void Update()
    {
        if (finished || data.timeLimit <= 0f) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null) timerText.text = Mathf.CeilToInt(Mathf.Max(timeLeft, 0f)).ToString();

        if (timeLeft <= 0f)
        {
            finished = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public bool GoalDone(int coins, int coinsTotal)
    {
        switch (data.goal)
        {
            case LevelGoal.CollectAllCoins:
                return coins >= coinsTotal;
            case LevelGoal.DefeatAllEnemies:
                enemies.RemoveAll(e => e == null);
                return enemies.Count == 0;
            default:
                return true;
        }
    }

    public void Complete()
    {
        if (finished) return;
        finished = true;

        if (intro != null) StopCoroutine(intro);
        winMenu.Finish();
    }

    void OnDrawGizmos()
    {
        if (levels == null || levels.Length == 0) return;
        LevelData d = levels[Mathf.Clamp(previewLevel, 0, levels.Length - 1)];
        if (d == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(d.playerStart, 0.5f);

        Gizmos.color = Color.red;
        foreach (EnemySpawn e in d.enemies)
        {
            Gizmos.DrawWireSphere(e.position, 0.5f);
            if (e.prefab != null && e.prefab.GetComponent<FoxPatrol>() != null)
                Gizmos.DrawLine(e.position + Vector2.left * e.patrolDistance, e.position + Vector2.right * e.patrolDistance);
        }

        Gizmos.color = Color.yellow;
        foreach (Vector2 c in d.coins) Gizmos.DrawWireSphere(c, 0.3f);
    }
}
