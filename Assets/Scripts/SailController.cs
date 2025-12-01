using UnityEngine;

public class SailController : MonoBehaviour
{
    public Transform sailMesh;
    public float rotateSpeed = 40f;
    public float raiseSpeed = 1f;

    public float minRaise = 10f;
    public float maxRaise = 100f;

    private float _raiseAmount;

    public float OpenPercent => _raiseAmount;

    private void Start()
    {
        if (sailMesh != null)
            _raiseAmount = sailMesh.localScale.y;
    }

    public void Rotate(float value)
    {
        transform.Rotate(0, value * rotateSpeed * Time.deltaTime, 0);
    }

    public void Raise(float value)
    {
        if (Mathf.Abs(value) < 0.1f) return;

        _raiseAmount += value * raiseSpeed * Time.deltaTime;
        _raiseAmount = Mathf.Clamp(_raiseAmount, minRaise, maxRaise);

        Vector3 s = sailMesh.localScale;
        sailMesh.localScale = new Vector3(s.x, _raiseAmount, s.z);
    }
}