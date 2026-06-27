using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class BattlePocongSFX : MonoBehaviour
{
    [Header("Combat Sounds")]
    [SerializeField] private AudioResource _dashSFX;
    [SerializeField] private AudioResource _attackHitSFX;

    private AudioManager _audioManager;

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void PlayDashSFX()
    {
        if (_audioManager != null && _dashSFX != null)
        {
            _audioManager.PlaySFX(_dashSFX); 
        }
    }

    public void PlayHitSFX()
    {
        if (_audioManager != null && _attackHitSFX != null)
        {
            _audioManager.PlaySFX(_attackHitSFX);
        }
    }
}
