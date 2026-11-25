using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundEntry
{
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 1f;
    public bool randomPitch = true;
}

// All sound settings in one asset: Assets/Resources/SoundLibrary.asset
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Game/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("Mixer")]
    public AudioMixer mixer;
    public AudioMixerGroup tracksGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup uiGroup;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot pausedSnapshot;
    public float pauseFadeTime = 0.3f;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip levelMusic;
    public float crossfadeTime = 1f;

    [Header("Variation")]
    public float minPitch = 0.92f;
    public float maxPitch = 1.08f;

    [Header("Player")]
    public SoundEntry jump;
    public SoundEntry shoot;
    public SoundEntry coin;
    public SoundEntry heal;
    public SoundEntry playerHurt;
    public SoundEntry lose;

    [Header("Enemies: wind-up plays before the attack")]
    public SoundEntry foxWindUp;
    public SoundEntry gooseWindUp;
    public SoundEntry mouseWindUp;
    public SoundEntry bossWindUp;
    public SoundEntry enemyHit;
    public SoundEntry enemyDeath;
    public SoundEntry bossHit;
    public SoundEntry bossShoot;
    public SoundEntry explosion;

    [Header("UI")]
    public SoundEntry uiClick;
    public SoundEntry levelComplete;
    public SoundEntry win;

    public SoundEntry Get(Sfx id)
    {
        switch (id)
        {
            case Sfx.Jump: return jump;
            case Sfx.Shoot: return shoot;
            case Sfx.Coin: return coin;
            case Sfx.Heal: return heal;
            case Sfx.PlayerHurt: return playerHurt;
            case Sfx.Lose: return lose;
            case Sfx.FoxWindUp: return foxWindUp;
            case Sfx.GooseWindUp: return gooseWindUp;
            case Sfx.MouseWindUp: return mouseWindUp;
            case Sfx.BossWindUp: return bossWindUp;
            case Sfx.EnemyHit: return enemyHit;
            case Sfx.EnemyDeath: return enemyDeath;
            case Sfx.BossHit: return bossHit;
            case Sfx.BossShoot: return bossShoot;
            case Sfx.Explosion: return explosion;
            case Sfx.UIClick: return uiClick;
            case Sfx.LevelComplete: return levelComplete;
            case Sfx.Win: return win;
        }
        return null;
    }
}
