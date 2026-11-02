using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public Hazard[] prefabs;
    public GameObject warningPrefab;
    public Boss boss;

    public float warnTime = 1f;
    public float interval = 2.5f;
    public float intervalPhase2 = 1.5f;

    public float minX = -8f;
    public float maxX = 8f;
    public float spawnY = 6f;
    public float markerY = 0f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);

        while (boss == null || !boss.IsDead)
        {
            yield return StartCoroutine(SpawnWithWarning());

            float wait = (boss != null && boss.IsPhaseTwo) ? intervalPhase2 : interval;
            yield return new WaitForSeconds(wait);
        }
    }

    IEnumerator SpawnWithWarning()
    {
        if (prefabs.Length == 0) yield break;

        float x = Random.Range(minX, maxX);

        if (boss != null)
        {
            int tries = 0;
            while (Mathf.Abs(x - boss.transform.position.x) < 2f && tries < 10)
            {
                x = Random.Range(minX, maxX);
                tries++;
            }
        }

        GameObject warning = null;
        if (warningPrefab != null)
        {
            warning = Instantiate(warningPrefab, new Vector3(x, markerY, 0), Quaternion.identity);
        }

        yield return new WaitForSeconds(warnTime);

        if (warning != null) Destroy(warning);

        Hazard randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
        Instantiate(randomPrefab, new Vector3(x, spawnY, 0), Quaternion.identity);
    }
}