using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Boss : MonoBehaviour
{
    public int maxHp = 8;
    public Slider hpBar;
    public WinMenu winMenu;
    public AudioClip hitClip;
    public AudioClip winClip;

    int hp;
    bool dead;
    SpriteRenderer sr;

    public bool IsPhaseTwo => hp <= maxHp / 2;
    public bool IsDead => dead;

    void Start()
    {
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
        if (hitClip != null) AudioSource.PlayClipAtPoint(hitClip, transform.position);

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
        if (winClip != null) AudioSource.PlayClipAtPoint(winClip, transform.position);
        Invoke(nameof(ShowWin), 1.5f);
    }

    void ShowWin()
    {
        if (winMenu != null) winMenu.ShowWin();
    }
}