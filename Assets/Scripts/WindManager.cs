using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance;

    public Transform windTransform;
    public float rotateSpeedMin = 5f;
    public float rotateSpeedMax = 20f;

    private float _currentRotateSpeed;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PickNewSpeed();
    }

    private void Update()
    {
        windTransform.Rotate(0, _currentRotateSpeed * Time.deltaTime, 0);
    }

    private void PickNewSpeed()
    {
        _currentRotateSpeed = Random.Range(rotateSpeedMin, rotateSpeedMax);
        Invoke(nameof(PickNewSpeed), 1f);
    }

    public Vector3 WindDirection => windTransform.forward;
}