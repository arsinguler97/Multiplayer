using UnityEngine;

public class ShipAutoMove : MonoBehaviour
{
    public SailController sail;
    public WindManager wind;

    public float minSpeed = 1f;
    public float maxSpeed = 10f;
    public float reverseSpeed = -2f;
    [SerializeField] private float speedSmoothTime = 1.5f;

    private float _currentSpeed;
    public float ShipSpeed => _currentSpeed;
    private float _speedVelocity;

    private void Update()
    {
        if (sail == null || wind == null)
            return;

        Vector3 windDir = wind.WindDirection;
        Vector3 sailDir = sail.sailMesh.forward;

        float angle = Vector3.Angle(sailDir, windDir);
        float angleFactor = Mathf.Cos(angle * Mathf.Deg2Rad);

        float sailAmount = sail.OpenPercent;

        float forwardSpeed = Mathf.Lerp(minSpeed, maxSpeed, sailAmount) * Mathf.Max(0, angleFactor);
        float backwardSpeed = reverseSpeed * Mathf.Clamp01(-angleFactor);

        float targetSpeed = forwardSpeed + backwardSpeed;
        _currentSpeed = Mathf.SmoothDamp(
            _currentSpeed,
            targetSpeed,
            ref _speedVelocity,
            speedSmoothTime
        );

        transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
    }
}
