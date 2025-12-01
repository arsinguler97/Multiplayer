using UnityEngine;

public class ShipAutoMove : MonoBehaviour
{
    [SerializeField] private float shipSpeed;

    public float ShipSpeed => shipSpeed;

    private void Update()
    {
        transform.position += transform.forward * (shipSpeed * Time.deltaTime);
    }
}