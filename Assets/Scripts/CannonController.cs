using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Transform horizontalPivot;
    public Transform verticalPivot;
    public float rotateSpeed = 40f;
    public float minV = -10f;
    public float maxV = 30f;
    public float minH = -30f;
    public float maxH = 30f;

    public Transform firePoint;
    public GameObject cannonballPrefab;
    public float shootForce = 20f;

    public void RotateHorizontal(float value)
    {
        float angle = horizontalPivot.localEulerAngles.y;
        if (angle > 180) angle -= 360;
        angle += value * rotateSpeed * Time.deltaTime;
        angle = Mathf.Clamp(angle, minH, maxH);
        horizontalPivot.localEulerAngles = new Vector3(0, angle, 0);
    }

    public void RotateVertical(float value)
    {
        float angle = verticalPivot.localEulerAngles.x;
        if (angle > 180) angle -= 360;
        angle -= value * rotateSpeed * Time.deltaTime;
        angle = Mathf.Clamp(angle, minV, maxV);
        verticalPivot.localEulerAngles = new Vector3(angle, 0, 0);
    }

    public void Fire()
    {
        GameObject ball = Instantiate(cannonballPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            ShipAutoMove ship = transform.root.GetComponent<ShipAutoMove>();
            float shipSpeed = ship != null ? ship.ShipSpeed : 0f;

            Vector3 inherit = transform.root.forward * shipSpeed;
            Vector3 forwardForce = firePoint.forward * shootForce;

            rb.linearVelocity = inherit + forwardForce;
        }
    }
}