using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    const string FileName = "save.json";
    const int CurrentVersion = 1;

    static SaveData data;

    public static bool LoadFailed { get; private set; }

    public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static SaveData Data
    {
        get
        {
            if (data == null) data = Load();
            return data;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnPlay()
    {
        data = null;
        LoadFailed = false;
    }

    static SaveData Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new SaveData();

            string json = File.ReadAllText(FilePath);
            SaveData loaded = JsonUtility.FromJson<SaveData>(json);
            if (loaded == null || loaded.version != CurrentVersion)
            {
                LoadFailed = true;
                return new SaveData();
            }

            loaded.unlockedLevels = Mathf.Max(1, loaded.unlockedLevels);
            if (!json.Contains("\"sfxVolume\"")) loaded.sfxVolume = 1f;   // save from week 11
            loaded.musicVolume = Mathf.Clamp01(loaded.musicVolume);
            loaded.sfxVolume = Mathf.Clamp01(loaded.sfxVolume);
            if (loaded.bestScores == null) loaded.bestScores = new System.Collections.Generic.List<int>();
            return loaded;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Save file is broken, using defaults. " + e.Message);
            LoadFailed = true;
            return new SaveData();
        }
    }

    public static void Save()
    {
        try
        {
            string temp = Path.Combine(Application.persistentDataPath, "save.tmp");
            File.WriteAllText(temp, JsonUtility.ToJson(Data, true));

            if (File.Exists(FilePath)) File.Replace(temp, FilePath, null);
            else File.Move(temp, FilePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not save. " + e.Message);
        }
    }

    public static bool IsUnlocked(int levelIndex)
    {
        return levelIndex < Data.unlockedLevels;
    }

    public static int BestScore(int levelIndex)
    {
        return levelIndex < Data.bestScores.Count ? Data.bestScores[levelIndex] : 0;
    }

    public static void CompleteLevel(int levelIndex, int score)
    {
        while (Data.bestScores.Count <= levelIndex) Data.bestScores.Add(0);
        if (score > Data.bestScores[levelIndex]) Data.bestScores[levelIndex] = score;

        Data.unlockedLevels = Mathf.Max(Data.unlockedLevels, levelIndex + 2);
        Save();
    }

    public static void SetMusicVolume(float volume)
    {
        Data.musicVolume = Mathf.Clamp01(volume);
    }

    public static void SetSfxVolume(float volume)
    {
        Data.sfxVolume = Mathf.Clamp01(volume);
    }

    public static void AcknowledgeLoadFailed()
    {
        LoadFailed = false;
    }

    public static void ResetProgress()
    {
        float music = Data.musicVolume;
        float sfx = Data.sfxVolume;
        data = new SaveData();
        data.musicVolume = music;
        data.sfxVolume = sfx;
        Save();
    }
}
