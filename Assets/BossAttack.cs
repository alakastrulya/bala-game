using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public Boss boss;
    public Transform target;
    public Transform firePoint;
    public Syringe syringePrefab;
    public Animator animator;
    public AudioClip shootClip;

    public float pause = 2.5f;
    public float pausePhase2 = 1.5f;
    public float warnTime = 0.8f;

    public int countPhase1 = 3;
    public int countPhase2 = 5;
    public float anglePhase1 = 20f;
    public float anglePhase2 = 25f;


    void Start()
    {
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(2f);

        while (!boss.IsDead)
        {
            if (animator != null) animator.SetTrigger("Attack");
            yield return new WaitForSeconds(warnTime);

            if (boss.IsDead) yield break;
            Fire();

            float wait = boss.IsPhaseTwo ? pausePhase2 : pause;
            yield return new WaitForSeconds(wait);
        }
    }

    void Fire()
    {
        Vector2 dir = (target.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        int count = boss.IsPhaseTwo ? countPhase2 : countPhase1;
        float step = boss.IsPhaseTwo ? anglePhase2 : anglePhase1;

        for (int i = 0; i < count; i++)
        {
            float offset = (i - (count - 1) / 2f) * step;
            Instantiate(syringePrefab, firePoint.position, Quaternion.Euler(0, 0, baseAngle + offset));
        }

        if (shootClip != null) AudioSource.PlayClipAtPoint(shootClip, firePoint.position);
    }
}