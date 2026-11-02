using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int hp = 3;
    private bool invincible = false;
    private SpriteRenderer sr;

    public AudioClip hurtClip;
    public AudioClip healClip;
    private AudioSource audioSource;
    public Image[] hearts;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        UpdateHeartsUI();
    }

    public void TakeDamage(int dmg)
    {
        if (invincible) return;

        hp -= dmg;
        Debug.Log("HP: " + hp);

        UpdateHeartsUI();

        if (audioSource != null && hurtClip != null)
        {
            audioSource.PlayOneShot(hurtClip);
        }

        if (hp <= 0)
        {
            RestartLevel();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > 3) hp = 3;
        Debug.Log("Healed! HP: " + hp);

        UpdateHeartsUI();
        
        if (audioSource != null && healClip != null)
        {
            audioSource.PlayOneShot(healClip);
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        invincible = true;

        for (int i = 0; i < 10; i++)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.2f);
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