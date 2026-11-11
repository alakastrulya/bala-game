using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHp = 2;
    [SerializeField] AudioClip hitClip;
    [SerializeField] float blinkTime = 0.3f;
    [SerializeField] float barWidth = 1f;
    [SerializeField] float barHeight = 0.12f;
    [SerializeField] float barOffsetY = 0.3f;
    [SerializeField] Color barColor = new Color(0.3f, 0.85f, 0.3f);

    int hp;
    SpriteRenderer sr;
    Transform barFill;

    void Awake()
    {
        hp = maxHp;
        sr = GetComponent<SpriteRenderer>();
        CreateBar();
    }

    public void TakeHit(int damage)
    {
        hp -= damage;
        if (hitClip != null) AudioSource.PlayClipAtPoint(hitClip, transform.position);

        if (hp <= 0)
        {
            Destroy(gameObject);
            return;
        }

        UpdateBar();
        StopAllCoroutines();
        StartCoroutine(Blink());
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