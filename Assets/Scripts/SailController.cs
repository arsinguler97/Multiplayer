using UnityEngine;
using System.Collections.Generic;

public class SailController : MonoBehaviour
{
    public Transform sailMesh;
    public float rotateSpeed = 40f;
    public float raiseSpeed = 1f;

    public float minRaise = 10f;
    public float maxRaise = 100f;

    private float _raiseAmount;
    private readonly HashSet<PlayerInputHandler> _activeUsers = new HashSet<PlayerInputHandler>();

    public float OpenPercent => (_raiseAmount - minRaise) / (maxRaise - minRaise);

    private void Start()
    {
        if (sailMesh != null)
            _raiseAmount = sailMesh.localScale.y;
    }

    private void Update()
    {
        float totalRotate = 0f;
        float totalRaise = 0f;

        foreach (PlayerInputHandler user in _activeUsers)
        {
            if (user == null) continue;
            totalRotate += user.SailRotateInput;
            totalRaise += user.SailRaiseInput;
        }

        if (Mathf.Abs(totalRotate) > 0.001f) Rotate(totalRotate);
        if (Mathf.Abs(totalRaise) > 0.001f) Raise(totalRaise);
    }

    public void RegisterSailUser(PlayerInputHandler user)
    {
        if (user == null) return;
        _activeUsers.Add(user);
    }

    public void UnregisterSailUser(PlayerInputHandler user)
    {
        if (user == null) return;
        _activeUsers.Remove(user);
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
