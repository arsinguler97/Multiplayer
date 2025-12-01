using UnityEngine;

public class OceanFollow : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}