using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -10);
    public float smooth = 5f;
    public float minX = 0f;
    public float maxX = 30f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 wanted = target.position + offset;
        wanted.x = Mathf.Clamp(wanted.x, minX, maxX);
        transform.position = Vector3.Lerp(transform.position, wanted, smooth * Time.deltaTime);
    }
}