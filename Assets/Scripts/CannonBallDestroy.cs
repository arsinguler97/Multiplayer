using UnityEngine;

public class CannonBallDestroy : MonoBehaviour
{
    private float _t;

    private void Update()
    {
        _t += Time.deltaTime;
        if (_t >= 5f) Destroy(gameObject);
    }
}