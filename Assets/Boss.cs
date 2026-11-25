using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    [SerializeField] BossStats stats;
    public BossStats Stats => stats;
    public Slider hpBar;
    public WinMenu winMenu;

    int hp;
    int maxHp;
    bool dead;
    SpriteRenderer sr;

    public bool IsPhaseTwo => hp <= maxHp / 2;
    public bool IsDead => dead;

    void Start()
    {
        if (winMenu == null) winMenu = FindAnyObjectByType<WinMenu>();
        maxHp = stats.maxHp;
        hp = maxHp;
        sr = GetComponent<SpriteRenderer>();
        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            hpBar.value = hp;
        }
    }

    public void TakeHit(int dmg)
    {
        if (dead) return;

        hp -= dmg;
        if (hpBar != null) hpBar.value = hp;
        AudioManager.Play(Sfx.BossHit);

        if (hp <= 0) Die();
        else StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    void Die()
    {
        dead = true;
        GetComponent<Collider2D>().enabled = false;
        AudioManager.Play(Sfx.EnemyDeath);
        Invoke(nameof(ShowWin), 1.5f);
    }

    void ShowWin()
    {
        int score = LevelCatalog.ScoreForCurrent(0);
        if (winMenu != null)
        {
            winMenu.FinishWithScore(score);
            return;
        }

        LevelCatalog.CompleteCurrent(score);

        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else SceneManager.LoadScene(0);
    }
}