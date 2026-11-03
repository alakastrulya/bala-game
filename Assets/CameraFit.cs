using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{

    public float targetAspect = 16f / 9f;

    private Camera cam;
    private float baseSize;
    private float lastAspect;

    void Awake()
    {
        cam = GetComponent<Camera>();
        baseSize = cam.orthographicSize;
        Apply();
    }

    void Update()
    {

        if (!Mathf.Approximately(cam.aspect, lastAspect))
            Apply();
    }

    void Apply()
    {
        lastAspect = cam.aspect;

        if (cam.aspect < targetAspect)
        {
            cam.orthographicSize = baseSize * targetAspect / cam.aspect;
        }
        else
        {

            cam.orthographicSize = baseSize;
        }
    }
}