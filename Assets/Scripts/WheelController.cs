using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform ship;
    public ShipAutoMove shipMove;

    public float minTurnSpeed = 5f;
    public float maxTurnSpeed = 40f;

    public void Turn(float value)
    {
        float absSpeed = Mathf.Abs(shipMove.ShipSpeed);

        float t = Mathf.InverseLerp(0f, shipMove.maxSpeed, absSpeed);
        float turnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, t);

        ship.Rotate(0, value * turnSpeed * Time.deltaTime, 0);
    }
}