using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelEntry
{
    public string title = "Level";
    public string startScene;
    public string gameScene;
    public int dataLevel = -1;

    [Header("Only for normal scenes (Data Level = -1)")]
    public float timeLimit = 90f;
    public int pointsPerCoin = 100;
    public int pointsPerSecond = 10;
}

[CreateAssetMenu(fileName = "LevelCatalog", menuName = "Game/Level Catalog")]
public class LevelCatalog : ScriptableObject
{
    public List<LevelEntry> levels = new List<LevelEntry>();

    static LevelCatalog instance;

    public static LevelCatalog Get
    {
        get
        {
            if (instance == null) instance = Resources.Load<LevelCatalog>("LevelCatalog");
            return instance;
        }
    }

    public int CurrentIndex()
    {
        string scene = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levels.Count; i++)
        {
            LevelEntry e = levels[i];
            if (e.gameScene != scene) continue;
            if (e.dataLevel < 0 || e.dataLevel == GameSession.LevelIndex) return i;
        }
        return -1;
    }

    public LevelEntry CurrentEntry()
    {
        int i = CurrentIndex();
        return i >= 0 ? levels[i] : null;
    }

    public static int ScoreForCurrent(int coins)
    {
        LevelEntry e = Get != null ? Get.CurrentEntry() : null;
        if (e == null) return 0;
        int seconds = LevelTimer.Instance != null ? LevelTimer.Instance.SecondsLeft : 0;
        return coins * e.pointsPerCoin + seconds * e.pointsPerSecond;
    }

    public static void CompleteCurrent(int score)
    {
        if (Get == null) return;
        int index = Get.CurrentIndex();
        if (index >= 0) SaveSystem.CompleteLevel(index, score);
    }

    public void Load(int index)
    {
        LevelEntry e = levels[index];
        Time.timeScale = 1f;
        GameSession.StartedFromMenu = true;
        GameSession.LevelIndex = Mathf.Max(0, e.dataLevel);
        SceneManager.LoadScene(string.IsNullOrEmpty(e.startScene) ? e.gameScene : e.startScene);
    }
}
