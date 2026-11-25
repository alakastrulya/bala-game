using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

// Tools > Audio > Fill Sound Library: finds the GameMix mixer and the clips in Assets/Audio
// and puts them into Assets/Resources/SoundLibrary.asset.
public static class AudioSetupTools
{
    const string LibraryPath = "Assets/Resources/SoundLibrary.asset";
    const string SfxFolder = "Assets/Audio/SFX";
    const string MusicFolder = "Assets/Audio/Music";

    [MenuItem("Tools/Audio/Fill Sound Library")]
    static void Fill()
    {
        SoundLibrary lib = AssetDatabase.LoadAssetAtPath<SoundLibrary>(LibraryPath);
        bool isNew = lib == null;
        if (isNew)
        {
            lib = ScriptableObject.CreateInstance<SoundLibrary>();
            AssetDatabase.CreateAsset(lib, LibraryPath);
        }

        FillMixer(lib);

        lib.menuMusic = AssetDatabase.LoadAssetAtPath<AudioClip>(MusicFolder + "/music_menu.wav");
        lib.levelMusic = AssetDatabase.LoadAssetAtPath<AudioClip>(MusicFolder + "/music_level.wav");

        lib.jump = Entry(lib.jump, "jump", isNew, 0.5f, true);
        lib.shoot = Entry(lib.shoot, "shoot", isNew, 0.6f, true);
        lib.coin = Entry(lib.coin, "coin", isNew, 0.7f, true);
        lib.heal = Entry(lib.heal, "heal", isNew, 0.8f, true);
        lib.playerHurt = Entry(lib.playerHurt, "player_hurt", isNew, 1f, true);
        lib.lose = Entry(lib.lose, "lose", isNew, 0.9f, false);
        lib.foxWindUp = Entry(lib.foxWindUp, "fox_windup", isNew, 1f, true);
        lib.gooseWindUp = Entry(lib.gooseWindUp, "goose_windup", isNew, 0.9f, true);
        lib.mouseWindUp = Entry(lib.mouseWindUp, "mouse_windup", isNew, 0.8f, true);
        lib.bossWindUp = Entry(lib.bossWindUp, "boss_windup", isNew, 0.9f, true);
        lib.enemyHit = Entry(lib.enemyHit, "enemy_hit", isNew, 0.9f, true);
        lib.enemyDeath = Entry(lib.enemyDeath, "enemy_death", isNew, 0.8f, true);
        lib.bossHit = Entry(lib.bossHit, "boss_hit", isNew, 0.9f, true);
        lib.bossShoot = Entry(lib.bossShoot, "boss_shoot", isNew, 0.7f, true);
        lib.explosion = Entry(lib.explosion, "explosion", isNew, 1f, true);
        lib.uiClick = Entry(lib.uiClick, "ui_click", isNew, 0.6f, false);
        lib.levelComplete = Entry(lib.levelComplete, "level_complete", isNew, 0.9f, false);
        lib.win = Entry(lib.win, "win", isNew, 1f, false);

        EditorUtility.SetDirty(lib);
        AssetDatabase.SaveAssets();
        Selection.activeObject = lib;
        Debug.Log("Sound Library filled: " + LibraryPath);
    }

    static void FillMixer(SoundLibrary lib)
    {
        if (lib.mixer == null)
        {
            foreach (string guid in AssetDatabase.FindAssets("t:AudioMixer"))
            {
                AudioMixer m = AssetDatabase.LoadAssetAtPath<AudioMixer>(AssetDatabase.GUIDToAssetPath(guid));
                if (m != null && m.name == "GameMix") lib.mixer = m;
            }
        }
        if (lib.mixer == null)
        {
            Debug.LogWarning("No AudioMixer called GameMix found. Create it first (step 2).");
            return;
        }

        foreach (AudioMixerGroup g in lib.mixer.FindMatchingGroups(string.Empty))
        {
            if (g.name == "Tracks") lib.tracksGroup = g;
            else if (g.name == "SFX") lib.sfxGroup = g;
            else if (g.name == "UI") lib.uiGroup = g;
        }
        lib.normalSnapshot = lib.mixer.FindSnapshot("Normal");
        lib.pausedSnapshot = lib.mixer.FindSnapshot("Paused");

        if (lib.tracksGroup == null) Debug.LogWarning("Mixer group Tracks not found.");
        if (lib.sfxGroup == null) Debug.LogWarning("Mixer group SFX not found.");
        if (lib.normalSnapshot == null || lib.pausedSnapshot == null) Debug.LogWarning("Snapshots Normal / Paused not found.");
    }

    // finds "name.wav" or "name_1.wav", "name_2.wav"...
    static SoundEntry Entry(SoundEntry e, string name, bool setDefaults, float volume, bool randomPitch)
    {
        if (e == null) e = new SoundEntry();

        List<AudioClip> clips = new List<AudioClip>();
        foreach (string guid in AssetDatabase.FindAssets("t:AudioClip", new[] { SfxFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string file = Path.GetFileNameWithoutExtension(path);
            if (file == name || (file.StartsWith(name + "_") && int.TryParse(file.Substring(name.Length + 1), out _)))
                clips.Add(AssetDatabase.LoadAssetAtPath<AudioClip>(path));
        }
        clips.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        e.clips = clips.ToArray();
        if (clips.Count == 0) Debug.LogWarning("No clip for " + name + " in " + SfxFolder);

        if (setDefaults)
        {
            e.volume = volume;
            e.randomPitch = randomPitch;
        }
        return e;
    }
}

// Import settings from the lecture: short SFX vs streamed music.
public class AudioImportRules : AssetPostprocessor
{
    void OnPreprocessAudio()
    {
        AudioImporter importer = (AudioImporter)assetImporter;
        AudioImporterSampleSettings s = importer.defaultSampleSettings;

        if (assetPath.StartsWith("Assets/Audio/SFX/"))
        {
            importer.forceToMono = true;
            s.loadType = AudioClipLoadType.DecompressOnLoad;
            s.compressionFormat = AudioCompressionFormat.ADPCM;
        }
        else if (assetPath.StartsWith("Assets/Audio/Music/"))
        {
            importer.forceToMono = false;
            s.loadType = AudioClipLoadType.Streaming;
            s.compressionFormat = AudioCompressionFormat.Vorbis;
            s.quality = 0.6f;
        }
        else return;

        importer.defaultSampleSettings = s;
    }
}
