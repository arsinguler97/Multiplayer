using UnityEngine;

public class ParabolaRenderer : MonoBehaviour
{
    [Header("Parabola Settings")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField, Min(2)] int segments = 20;
    public void DrawTrajectory(Vector3 start, Vector3 velocity, Vector3 gravity, float endTime)
    {
        if (lineRenderer == null || segments < 2)
        {
            return;
        }

        lineRenderer.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float t = i / (segments - 1f);
            float time = t * endTime;
            Vector3 point = start + velocity * time + 0.5f * gravity * time * time;
            lineRenderer.SetPosition(i, point);
        }
    }
}
