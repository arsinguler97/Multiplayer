using UnityEngine;

public class CompassUI : MonoBehaviour
{
    [SerializeField] private RectTransform compassImage;
    [SerializeField] private Transform shipTransform;
    [SerializeField] private bool invertDirection = true;
    [SerializeField] private float angleOffset = 0f;
    [SerializeField] private string playerTag = "PlayerShip";

    private void Awake()
    {
        if (compassImage == null)
        {
            compassImage = transform as RectTransform;
        }
    }

    private void LateUpdate()
    {
        if (compassImage == null) return;

        if (shipTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null) shipTransform = player.transform;
        }

        if (shipTransform == null) return;

        float shipYaw = shipTransform.eulerAngles.y;
        float z = invertDirection ? -shipYaw : shipYaw;
        z += angleOffset;

        compassImage.localEulerAngles = new Vector3(0f, 0f, z);
    }
}
