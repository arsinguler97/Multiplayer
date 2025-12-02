using UnityEngine;

public class WindParticle : MonoBehaviour
{
    public Transform windDirectionRoot;

    void Update()
    {
        transform.rotation = windDirectionRoot.rotation;
    }
}