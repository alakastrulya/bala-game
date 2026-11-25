using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    int hp;
    PlayerStats stats;
    private bool invincible = false;
    private SpriteRenderer sr;

    public Image[] hearts;

    void Awake()
    {
        stats = GetComponent<Player2D>().Stats;
        hp = stats.maxHp;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateHeartsUI();
    }

    public void TakeDamage(int dmg)
    {
        if (invincible) return;

        hp -= dmg;
        Debug.Log("HP: " + hp);

        UpdateHeartsUI();

        if (hp <= 0)
        {
            AudioManager.Play(Sfx.Lose);
            RestartLevel();
        }
        else
        {
            AudioManager.Play(Sfx.PlayerHurt);
            StartCoroutine(InvincibilityRoutine());
        }
    }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > stats.maxHp) hp = stats.maxHp;
        Debug.Log("Healed! HP: " + hp);

        UpdateHeartsUI();
        AudioManager.Play(Sfx.Heal);
    }

    IEnumerator InvincibilityRoutine()
    {
        invincible = true;

        const float blink = 0.2f;
        for (float t = 0f; t < stats.invincibleTime; t += blink)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blink);
        }
        if (sr != null) sr.enabled = true;
        invincible = false;
    }

    void RestartLevel()
    {
        Debug.Log("Game Over! Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {

                if (i < hp)
                {
                    hearts[i].gameObject.SetActive(true);
                }
                else
                {
                    hearts[i].gameObject.SetActive(false);
                }
            }
        }
    }
}