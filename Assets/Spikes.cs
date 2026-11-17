using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Health h = other.GetComponent<Health>();
        if (h != null) h.TakeDamage(other.GetComponent<Player2D>().Items.spikesDamage);
    }
}