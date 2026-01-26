using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance;

    [Header("References")]
    public Transform windTransform;

    [Header("Direction")]
    public float directionHoldMin = 10f;
    public float directionHoldMax = 20f;
    public float directionTransitionMin = 2f;
    public float directionTransitionMax = 3f;

    [Header("Strength")]
    public float strengthMin = 0.5f;
    public float strengthMax = 2f;
    public float strengthHoldMin = 6f;
    public float strengthHoldMax = 14f;
    public float strengthTransitionMin = 1f;
    public float strengthTransitionMax = 2.5f;

    private float _currentStrength;
    private Vector3 _currentDirection;

    // Expose current wind direction/strength for external access
    public Vector3 WindDirection => _currentDirection;
    public float WindStrength => _currentStrength;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Initialize cached direction/strength and start coroutines
        _currentDirection = windTransform.forward.normalized;
        _currentStrength = Random.Range(strengthMin, strengthMax);

        StartCoroutine(DirectionRoutine());
        StartCoroutine(StrengthRoutine());
    }

    private IEnumerator DirectionRoutine()
    {
        while (true)
        {
            float holdTime = Random.Range(directionHoldMin, directionHoldMax);
            yield return new WaitForSeconds(holdTime);

            float targetYaw = Random.Range(0f, 360f);
            Quaternion startRot = windTransform.rotation;
            Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);

            float transitionTime = Random.Range(directionTransitionMin, directionTransitionMax);
            float elapsed = 0f;

            while (elapsed < transitionTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transitionTime);
                windTransform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                yield return null;
            }

            windTransform.rotation = targetRot;
            _currentDirection = windTransform.forward.normalized;
        }
    }

    private IEnumerator StrengthRoutine()
    {
        while (true)
        {
            float holdTime = Random.Range(strengthHoldMin, strengthHoldMax);
            yield return new WaitForSeconds(holdTime);

            float targetStrength = Random.Range(strengthMin, strengthMax);
            float startStrength = _currentStrength;
            float transitionTime = Random.Range(strengthTransitionMin, strengthTransitionMax);
            float elapsed = 0f;

            while (elapsed < transitionTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transitionTime);
                _currentStrength = Mathf.Lerp(startStrength, targetStrength, t);
                yield return null;
            }

            _currentStrength = targetStrength;
        }
    }
}
