using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// One object for all sound. Created by code before the first scene, lives through scene loads.
public class AudioManager : MonoBehaviour
{
    const int SfxVoices = 8;
    const float MinVolume = 0.0001f;   // Log10(0) is minus infinity; 0.0001 is exactly -80 dB
    const string MusicParam = "MusicVol";
    const string SfxParam = "SFXVol";

    public static AudioManager Instance { get; private set; }

    SoundLibrary lib;
    AudioSource musicA, musicB, currentMusic;
    AudioSource[] voices;
    AudioSource uiSource;
    int nextVoice;
    readonly Dictionary<Sfx, int> lastClip = new Dictionary<Sfx, int>();

    // ---------- start-up ----------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic()
    {
        Instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        SoundLibrary library = Resources.Load<SoundLibrary>("SoundLibrary");
        if (library == null)
        {
            Debug.LogWarning("No Assets/Resources/SoundLibrary asset, the game will be silent.");
            return;
        }

        GameObject go = new GameObject("AudioManager");
        DontDestroyOnLoad(go);
        go.AddComponent<AudioManager>().Init(library);
    }

    void Init(SoundLibrary library)
    {
        Instance = this;
        lib = library;

        musicA = NewSource(lib.tracksGroup, true);
        musicB = NewSource(lib.tracksGroup, true);
        currentMusic = musicA;

        voices = new AudioSource[SfxVoices];
        for (int i = 0; i < SfxVoices; i++) voices[i] = NewSource(lib.sfxGroup, false);
        uiSource = NewSource(lib.uiGroup != null ? lib.uiGroup : lib.sfxGroup, false);

        // pause sets Time.timeScale = 0, so the mixer must run on real time
        if (lib.mixer != null) lib.mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    AudioSource NewSource(AudioMixerGroup group, bool loop)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.outputAudioMixerGroup = group;
        s.playOnAwake = false;
        s.loop = loop;
        s.spatialBlend = 0f;   // 2D game: same volume everywhere
        return s;
    }

    void Start()
    {
        // SetFloat does not work in Awake, so saved volumes are applied here
        ApplyVolumes();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPaused(false);
        PlayMusic(IsLevelScene(scene.name) ? lib.levelMusic : lib.menuMusic);
        AddClickSounds(scene);
    }

    static bool IsLevelScene(string sceneName)
    {
        LevelCatalog catalog = LevelCatalog.Get;
        if (catalog == null) return false;
        foreach (LevelEntry e in catalog.levels)
            if (e.gameScene == sceneName) return true;
        return false;
    }

    void AddClickSounds(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Button b in root.GetComponentsInChildren<Button>(true))
            {
                b.onClick.RemoveListener(PlayClick);
                b.onClick.AddListener(PlayClick);
            }
        }
    }

    static void PlayClick()
    {
        Play(Sfx.UIClick);
    }

    // ---------- music ----------
    void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (currentMusic.clip == clip && currentMusic.isPlaying) return;   // same track keeps playing on Retry

        AudioSource next = currentMusic == musicA ? musicB : musicA;
        next.clip = clip;
        next.volume = 0f;
        next.Play();

        StopAllCoroutines();
        StartCoroutine(Crossfade(currentMusic, next, lib.crossfadeTime));
        currentMusic = next;
    }

    IEnumerator Crossfade(AudioSource from, AudioSource to, float time)
    {
        float fromStart = from.volume;
        for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / time)
        {
            from.volume = fromStart * (1f - t);
            to.volume = t;
            yield return null;
        }
        from.Stop();
        to.volume = 1f;
    }

    // ---------- sound effects ----------
    public static void Play(Sfx id)
    {
        if (Instance != null) Instance.PlayEvent(id);
    }

    void PlayEvent(Sfx id)
    {
        SoundEntry e = lib.Get(id);
        if (e == null || e.clips == null || e.clips.Length == 0) return;

        int i = Random.Range(0, e.clips.Length);
        if (e.clips.Length > 1 && lastClip.TryGetValue(id, out int last) && i == last)
            i = (i + 1) % e.clips.Length;   // never the same clip twice in a row
        lastClip[id] = i;
        if (e.clips[i] == null) return;

        AudioSource src = id == Sfx.UIClick ? uiSource : FreeVoice();
        src.pitch = e.randomPitch ? Random.Range(lib.minPitch, lib.maxPitch) : 1f;
        src.PlayOneShot(e.clips[i], e.volume);
    }

    AudioSource FreeVoice()
    {
        for (int k = 0; k < voices.Length; k++)
        {
            AudioSource v = voices[(nextVoice + k) % voices.Length];
            if (!v.isPlaying)
            {
                nextVoice = (nextVoice + k + 1) % voices.Length;
                return v;
            }
        }
        AudioSource oldest = voices[nextVoice];
        nextVoice = (nextVoice + 1) % voices.Length;
        return oldest;
    }

    // ---------- pause ----------
    public static void SetPaused(bool paused)
    {
        if (Instance == null) return;
        SoundLibrary l = Instance.lib;
        AudioMixerSnapshot snap = paused ? l.pausedSnapshot : l.normalSnapshot;
        if (snap != null) snap.TransitionTo(l.pauseFadeTime);
    }

    // ---------- volume ----------
    public static float ToDecibels(float value)
    {
        return Mathf.Log10(Mathf.Max(value, MinVolume)) * 20f;
    }

    public static void ApplyVolumes()
    {
        if (Instance == null || Instance.lib.mixer == null) return;
        Instance.lib.mixer.SetFloat(MusicParam, ToDecibels(SaveSystem.Data.musicVolume));
        Instance.lib.mixer.SetFloat(SfxParam, ToDecibels(SaveSystem.Data.sfxVolume));
    }
}
