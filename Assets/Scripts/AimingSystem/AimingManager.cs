using UnityEngine;

public class AimingManager : MonoBehaviour
{
    [SerializeField] Transform cannonPosition;
    [SerializeField] Transform targetMesh;
    [SerializeField] float targetMoveSpeed = 5f;
    ParabolaRenderer parabolaRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parabolaRenderer = GetComponentInChildren<ParabolaRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParabola();
    }


    public void UpdateParabola()
    {
        if (parabolaRenderer == null || cannonPosition == null || targetMesh == null) return;
        parabolaRenderer.DrawParabola(cannonPosition.position, targetMesh.position);
    }

    public void MoveTarget(float horizontal, float vertical)
    {
        if (cannonPosition == null || targetMesh == null) return;
        if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f)) return;

        Vector3 right = Vector3.ProjectOnPlane(cannonPosition.right, Vector3.up).normalized;
        Vector3 forward = Vector3.ProjectOnPlane(cannonPosition.forward, Vector3.up).normalized;

        Vector3 delta = (right * horizontal + forward * vertical) * targetMoveSpeed * Time.deltaTime;
        Vector3 pos = targetMesh.position + delta;
        targetMesh.position = pos;

        UpdateParabola();
    }
}
