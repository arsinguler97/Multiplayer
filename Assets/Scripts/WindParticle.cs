using UnityEngine;
using UnityEngine.VFX;

public class WindParticle : MonoBehaviour
{
    private WindManager _windManager;
    [SerializeField] VisualEffect windEffect;

    private void Start()
    {
        _windManager = WindManager.Instance;
    }

    void Update()
    {
        Vector3 windDirection = _windManager.WindDirection * _windManager.WindStrength;
        windEffect.SetVector3("WindDirection", windDirection);
    }
}
