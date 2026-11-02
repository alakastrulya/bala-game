using UnityEngine;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float cooldown = 0.3f;
    float nextShot = 0f;

    public AudioClip shootClip;
    private AudioSource audioSource;

    public Slider cooldownSlider;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = cooldown;
            cooldownSlider.value = cooldown;
        }
    }

    void Update()
    {
        if (cooldownSlider != null)
        {
            float timeRemaining = nextShot - Time.time;
            cooldownSlider.value = cooldown - Mathf.Clamp(timeRemaining, 0f, cooldown);
        }
    }

    public void Shoot()
    {
        if (Time.time < nextShot) return;
        nextShot = Time.time + cooldown;

        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0f;
        }

        if (audioSource != null && shootClip != null)
        {
            audioSource.PlayOneShot(shootClip);
        }

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        if (transform.localScale.x < 0)
        {
            bullet.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}