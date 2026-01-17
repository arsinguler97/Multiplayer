using UnityEngine;

public class EnemyCannonController : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject cannonballPrefab;
    [SerializeField] private float shootForce = 20f;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private bool useBallisticArc = true;
    [SerializeField] private float launchAngleDegrees = 25f;

    public Transform FirePoint => firePoint;

    public void Fire()
    {
        if (firePoint == null || cannonballPrefab == null) return;

        GameObject ball = Instantiate(cannonballPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * shootForce;
        }
    }

    public void FireAt(Vector3 targetPosition)
    {
        if (firePoint == null || cannonballPrefab == null) return;

        Vector3 dir;
        float speed;
        if (!TryGetBallisticShot(targetPosition, out dir, out speed))
        {
            dir = (targetPosition - firePoint.position).normalized;
            speed = shootForce;
            if (dir.sqrMagnitude < 0.01f) dir = firePoint.forward;
        }

        GameObject ball = Instantiate(cannonballPrefab, firePoint.position, Quaternion.LookRotation(dir, Vector3.up));
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * speed;
        }
    }

    private bool TryGetBallisticShot(Vector3 targetPosition, out Vector3 dir, out float speed)
    {
        dir = firePoint != null ? firePoint.forward : Vector3.forward;
        speed = shootForce;

        if (!useBallisticArc || firePoint == null) return false;

        Vector3 toTarget = targetPosition - firePoint.position;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float xz = toTargetXZ.magnitude;
        float y = toTarget.y;

        if (xz < 0.01f) return false;

        float angleRad = Mathf.Deg2Rad * Mathf.Clamp(launchAngleDegrees, 1f, 89f);
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        float g = -Physics.gravity.y;

        float denom = 2f * cos * cos * (xz * Mathf.Tan(angleRad) - y);
        if (denom <= 0.001f) return false;

        float v2 = (g * xz * xz) / denom;
        if (v2 <= 0.001f) return false;

        speed = Mathf.Sqrt(v2);
        Vector3 dirXZ = toTargetXZ.normalized;
        dir = dirXZ * cos + Vector3.up * sin;
        return true;
    }

    public void AimAt(Vector3 targetPosition, float rotateSpeed)
    {
        Transform pivot = aimPivot != null ? aimPivot : firePoint;
        if (pivot == null) return;

        Vector3 toTarget = targetPosition - pivot.position;
        if (toTarget.sqrMagnitude < 0.01f) return;

        Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        pivot.rotation = Quaternion.RotateTowards(pivot.rotation, desired, rotateSpeed * Time.deltaTime);
    }
}
