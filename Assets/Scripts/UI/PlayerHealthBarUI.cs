using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged += OnHealthChanged;
            OnHealthChanged(playerHealth.Current, playerHealth.Max);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;
    }
}
