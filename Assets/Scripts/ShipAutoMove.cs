using UnityEngine;

public class ShipAutoMove : MonoBehaviour
{
    public SailController sail;

    public float minSpeed = 1f;
    public float maxSpeed = 10f;

    private float _currentSpeed;
    public float ShipSpeed => _currentSpeed;

    private void Update()
    {
        float sailAmount = sail != null ? sail.OpenPercent : 0f;

        _currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, sailAmount);

        transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
    }
}