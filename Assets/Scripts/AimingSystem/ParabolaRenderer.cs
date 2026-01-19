using UnityEngine;

public class ParabolaRenderer : MonoBehaviour
{
    [Header("Parabola Settings")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField, Min(2)] int segments = 20;
    [SerializeField] float baseHeight = 1f;
    [SerializeField] float heightPerUnit = 0.25f;
    [SerializeField] float maxHeight = 6f;
    [SerializeField] Vector3 upDirection = Vector3.up;

    public void DrawParabola(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null || segments < 2)
        {
            return;
        }

        Vector3 up = upDirection.sqrMagnitude > 0f ? upDirection.normalized : Vector3.up;
        float distance = Vector3.Distance(start, end);
        float height = Mathf.Min(baseHeight + distance * heightPerUnit, maxHeight);

        lineRenderer.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float t = i / (segments - 1f);
            Vector3 point = Vector3.Lerp(start, end, t);
            float arc = 4f * height * t * (1f - t);
            point += up * arc;
            lineRenderer.SetPosition(i, point);
        }
    }
}
