using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -10);
    public float smooth = 5f;

    public float levelLeft = -10f;
    public float levelRight = 40f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        float halfHeight = cam.orthographic
        ? cam.orthographicSize
        : Mathf.Abs(transform.position.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        float halfWidth = halfHeight * cam.aspect;

        float minX = levelLeft + halfWidth;
        float maxX = levelRight - halfWidth;

        if (minX > maxX)
        {
            minX = (levelLeft + levelRight) / 2f;
            maxX = minX;
        }

        Vector3 wanted = target.position + offset;
        wanted.x = Mathf.Clamp(wanted.x, minX, maxX);
        transform.position = Vector3.Lerp(transform.position, wanted, smooth * Time.deltaTime);
    }
}