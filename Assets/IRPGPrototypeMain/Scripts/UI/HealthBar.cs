using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBarSlider;
    [SerializeField] private Slider _lerpBarSlider;
    private float _maxHealth = 100.0f;
    private float _currentHealth;
    private float _lerpSpeed = 0.05f;

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    void Update()
    {
        if (_healthBarSlider.value != _currentHealth) _healthBarSlider.value = _currentHealth;
        if (_healthBarSlider.value != _lerpBarSlider.value) _lerpBarSlider.value = Mathf.Lerp(_lerpBarSlider.value, _currentHealth, _lerpSpeed);
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
    }

    public void TakeHealing(float amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
    }

    public void SetupHealthBar(float _normalValue)
    {
        _currentHealth = _normalValue;
    }
}
