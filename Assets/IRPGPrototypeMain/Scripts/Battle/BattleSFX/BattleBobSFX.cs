using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class BattleBobSFX : MonoBehaviour
{
    [Header("Combat Sounds")]
    [SerializeField] private AudioResource _dashSFX;
    [SerializeField] private AudioResource _attackHitSFX;
    [SerializeField] private AudioResource _healSFX;
    [SerializeField] private AudioResource _spellCastSFX;

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

    public void PlayHealSFX()
    {
        if (_audioManager != null && _healSFX != null)
        {
            _audioManager.PlaySFX(_healSFX);
        }
    }

    public void PlaySpellCastSfx()
    {
        if (_audioManager != null && _spellCastSFX != null)
        {
            _audioManager.PlaySFX(_spellCastSFX);
        }
    }
}
