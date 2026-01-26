using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool lockYAxis = true;

    private void LateUpdate()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null) return;

        Vector3 toCam = cam.transform.position - transform.position;
        if (lockYAxis)
        {
            toCam.y = 0f;
        }

        if (toCam.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.LookRotation(toCam.normalized, Vector3.up);
    }
}
