using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBarSlider;
    [SerializeField] private Slider _lerpBarSlider;

    private float _lerpSpeed = 0.05f;

    // void Update()
    // {
    //     if (_healthBarSlider.value != _currentHealth) _healthBarSlider.value = _currentHealth;
    //     if (_healthBarSlider.value != _lerpBarSlider.value) _lerpBarSlider.value = Mathf.Lerp(_lerpBarSlider.value, _currentHealth, _lerpSpeed);
    // }

    // public void TakeDamage(float damage)
    // {
    //     _currentHealth -= damage;
    // }

    // public void TakeHealing(float amount)
    // {
    //     _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
    // }

    // public void SetupHealthBar(float _normalValue)
    // {
    //     _currentHealth = _normalValue;
    // }

    void Update()
    {
        // Smoothly lerp the secondary bar to catch up with the main instant-snap bar
        if (_lerpBarSlider != null && _healthBarSlider != null && _lerpBarSlider.value != _healthBarSlider.value) 
        {
            _lerpBarSlider.value = Mathf.Lerp(_lerpBarSlider.value, _healthBarSlider.value, _lerpSpeed);
        }
    }

    public void SetupHealthBar(float currentHealth, float maxHealth)
    {
        if (_healthBarSlider != null)
        {
            _healthBarSlider.maxValue = maxHealth;
            _healthBarSlider.value = currentHealth;
        }

        if (_lerpBarSlider != null)
        {
            _lerpBarSlider.maxValue = maxHealth;
            _lerpBarSlider.value = currentHealth;
        }
    }

    public void UpdateHealth(float newCurrentHealth)
    {
        if (_healthBarSlider != null)
        {
            _healthBarSlider.value = newCurrentHealth;
        }
    }
}
