using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player")) c.transform.SetParent(transform);
    }

    void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player")) c.transform.SetParent(null);
    }
}