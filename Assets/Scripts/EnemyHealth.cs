using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHp = 2;
    [SerializeField] AudioClip hitClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] float deathJump = 5f;
    [SerializeField] float deathTime = 1f;
    [SerializeField] float blinkTime = 0.3f;
    [SerializeField] float barWidth = 1f;
    [SerializeField] float barHeight = 0.12f;
    [SerializeField] float barOffsetY = 0.3f;
    [SerializeField] Color barColor = new Color(0.3f, 0.85f, 0.3f);

    int hp;
    SpriteRenderer sr;
    Transform barFill;
    bool dead;

    void Awake()
    {
        hp = maxHp;
        sr = GetComponent<SpriteRenderer>();
        CreateBar();
    }

    public void TakeHit(int damage)
    {
        if (dead) return;
        hp -= damage;

        if (hp <= 0)
        {
            Die();
            return;
        }

        if (hitClip != null) AudioSource.PlayClipAtPoint(hitClip, transform.position);
        UpdateBar();
        StopAllCoroutines();
        StartCoroutine(Blink());

        EnemyBase ai = GetComponent<EnemyBase>();
        if (ai != null) ai.Alert();
    }

    void Die()
    {
        dead = true;
        StopAllCoroutines();
        if (deathClip != null) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        EnemyBase ai = GetComponent<EnemyBase>();
        if (ai != null) ai.enabled = false;
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;
        foreach (Collider2D c in GetComponents<Collider2D>()) c.enabled = false;
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            if (r != sr) r.enabled = false;

        sr.enabled = true;
        sr.color = Color.white;
        sr.flipY = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = Mathf.Max(rb.gravityScale, 1f);
        rb.linearVelocity = new Vector2(0f, deathJump);

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        for (float t = 0f; t < deathTime; t += Time.deltaTime)
        {
            sr.color = new Color(1f, 1f, 1f, 1f - t / deathTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator Blink()
    {
        for (float t = 0f; t < blinkTime; t += 0.06f)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.06f);
        }
        sr.enabled = true;
    }

    void CreateBar()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        Sprite pixel = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0f, 0.5f), 1f);

        Vector3 s = transform.lossyScale;
        Transform bar = new GameObject("HpBar").transform;
        bar.SetParent(transform, false);
        bar.localScale = new Vector3(1f / Mathf.Abs(s.x), 1f / Mathf.Abs(s.y), 1f);
        float top = sr.bounds.extents.y + barOffsetY;
        bar.localPosition = new Vector3(0f, top / Mathf.Abs(s.y), 0f);

        MakePart(bar, pixel, Color.black, sr.sortingOrder + 1);
        barFill = MakePart(bar, pixel, barColor, sr.sortingOrder + 2);
        UpdateBar();
    }

    Transform MakePart(Transform bar, Sprite pixel, Color color, int order)
    {
        GameObject go = new GameObject("Part");
        go.transform.SetParent(bar, false);
        go.transform.localPosition = new Vector3(-barWidth / 2f, 0f, 0f);
        go.transform.localScale = new Vector3(barWidth, barHeight, 1f);

        SpriteRenderer r = go.AddComponent<SpriteRenderer>();
        r.sprite = pixel;
        r.color = color;
        r.sortingLayerID = sr.sortingLayerID;
        r.sortingOrder = order;
        return go.transform;
    }

    void UpdateBar()
    {
        barFill.localScale = new Vector3(barWidth * hp / maxHp, barHeight, 1f);
    }
}