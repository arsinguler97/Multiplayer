using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.HealthChanged += OnHealthChanged;
            OnHealthChanged(enemyHealth.Current, enemyHealth.Max);
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.HealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;
    }
}
