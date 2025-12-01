using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform ship;
    public float turnSpeed = 40f;

    public void Turn(float value)
    {
        ship.Rotate(0, value * turnSpeed * Time.deltaTime, 0);
    }
}