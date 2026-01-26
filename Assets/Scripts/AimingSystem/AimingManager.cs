using UnityEngine;

public class AimingManager : MonoBehaviour
{
    [SerializeField] Transform cannonPosition;
    [SerializeField] Transform targetMesh;
    [SerializeField] CannonController cannon;
    [SerializeField] ShipAutoMove shipMove;
    [SerializeField] Transform seaSurface;
    [SerializeField] float seaLevel = 0f;
    [SerializeField] float maxTime = 5f;
    [SerializeField] bool useCustomGravity = false;
    [SerializeField] Vector3 customGravity = new Vector3(0f, -9.81f, 0f);
    ParabolaRenderer parabolaRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parabolaRenderer = GetComponentInChildren<ParabolaRenderer>();
        if (cannon == null)
            cannon = GetComponentInParent<CannonController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParabola();
    }


    public void UpdateParabola()
    {
        if (parabolaRenderer == null || cannonPosition == null || targetMesh == null) return;

        Vector3 velocity = GetInitialVelocity();
        Vector3 gravity = useCustomGravity ? customGravity : Physics.gravity;
        float hitTime;
        bool hasHit = TryGetSeaHitTime(cannonPosition.position, velocity, gravity, GetSeaLevel(), maxTime, out hitTime);

        float endTime = hasHit ? hitTime : maxTime;
        parabolaRenderer.DrawTrajectory(cannonPosition.position, velocity, gravity, endTime);

        Vector3 impactPoint = cannonPosition.position + velocity * endTime + 0.5f * gravity * endTime * endTime;
        targetMesh.position = impactPoint;
    }

    public void MoveTarget(float horizontal, float vertical)
    {
        if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f)) return;
        UpdateParabola();
    }

    private Vector3 GetInitialVelocity()
    {
        float speed = cannon != null ? cannon.shootForce : 0f;
        Vector3 forward = cannonPosition.forward;

        ShipAutoMove ship = shipMove;
        if (ship == null)
            ship = cannonPosition.root.GetComponent<ShipAutoMove>();

        float shipSpeed = ship != null ? ship.ShipSpeed : 0f;
        Vector3 inherit = cannonPosition.root.forward * shipSpeed;

        return forward * speed + inherit;
    }

    private float GetSeaLevel()
    {
        if (seaSurface != null)
            return seaSurface.position.y;
        return seaLevel;
    }

    private static bool TryGetSeaHitTime(Vector3 start, Vector3 velocity, Vector3 gravity, float yPlane, float maxT, out float tHit)
    {
        float a = 0.5f * gravity.y;
        float b = velocity.y;
        float c = start.y - yPlane;

        tHit = maxT;

        float discriminant = b * b - 4f * a * c;
        if (Mathf.Abs(a) < 0.0001f)
        {
            if (Mathf.Abs(b) < 0.0001f)
                return false;

            float hitLinear = -c / b;
            if (hitLinear > 0f && hitLinear <= maxT)
            {
                tHit = hitLinear;
                return true;
            }
            return false;
        }

        if (discriminant < 0f)
            return false;

        float sqrt = Mathf.Sqrt(discriminant);
        float t1 = (-b - sqrt) / (2f * a);
        float t2 = (-b + sqrt) / (2f * a);

        float hitCandidate = float.MaxValue;
        if (t1 > 0f) hitCandidate = t1;
        if (t2 > 0f && t2 < hitCandidate) hitCandidate = t2;

        if (hitCandidate > 0f && hitCandidate <= maxT)
        {
            tHit = hitCandidate;
            return true;
        }

        return false;
    }
}
