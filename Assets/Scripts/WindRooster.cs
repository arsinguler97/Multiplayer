using UnityEngine;

public class WindDirectionFollower : MonoBehaviour
{
    [SerializeField] private WindManager windManager;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool invertDirection;
    [SerializeField] private float yawOffset;

    private void Awake()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }
    }

    private void LateUpdate()
    {
        WindManager manager = windManager != null ? windManager : WindManager.Instance;
        if (manager == null || targetTransform == null) return;

        Vector3 dir = manager.windTransform != null
            ? manager.windTransform.forward
            : manager.WindDirection;
        if (dir.sqrMagnitude < 0.0001f) return;

        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        if (invertDirection) yaw = -yaw;
        yaw += yawOffset;

        Vector3 euler = targetTransform.eulerAngles;
        euler.y = yaw;
        targetTransform.eulerAngles = euler;
    }
}
