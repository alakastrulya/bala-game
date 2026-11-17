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

    BossStats stats;


    void Start()
    {
        stats = boss.Stats;
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(stats.startDelay);

        while (!boss.IsDead)
        {
            if (animator != null) animator.SetTrigger("Attack");
            yield return new WaitForSeconds(stats.warnTime);

            if (boss.IsDead) yield break;
            Fire();

            float wait = boss.IsPhaseTwo ? stats.pausePhase2 : stats.pause;
            yield return new WaitForSeconds(wait);
        }
    }

    void Fire()
    {
        Vector2 dir = (target.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        int count = boss.IsPhaseTwo ? stats.countPhase2 : stats.countPhase1;
        float step = boss.IsPhaseTwo ? stats.anglePhase2 : stats.anglePhase1;

        for (int i = 0; i < count; i++)
        {
            float offset = (i - (count - 1) / 2f) * step;
            Syringe s = Instantiate(syringePrefab, firePoint.position, Quaternion.Euler(0, 0, baseAngle + offset));
            s.speed = stats.syringeSpeed;
        }

        if (shootClip != null) AudioSource.PlayClipAtPoint(shootClip, firePoint.position);
    }
}