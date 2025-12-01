using UnityEngine;

public class WindParticle : MonoBehaviour
{
    public Transform windTransform;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, windTransform.eulerAngles.y, 0);
    }
}