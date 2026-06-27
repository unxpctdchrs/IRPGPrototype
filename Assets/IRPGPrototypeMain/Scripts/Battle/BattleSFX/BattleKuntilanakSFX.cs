using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class BattleKuntilanakSFX : MonoBehaviour
{
    [Header("Combat Sounds")]
    [SerializeField] private AudioResource _explosionSFX;
    [SerializeField] private AudioResource _spellCastSFX;

    private AudioManager _audioManager;

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void PlayExplosionSFX()
    {
        if (_audioManager != null && _explosionSFX != null)
        {
            _audioManager.PlaySFX(_explosionSFX); 
        }
    }

    public void PlaySpellCastSFX()
    {
        if (_audioManager != null && _spellCastSFX != null)
        {
            _audioManager.PlaySFX(_spellCastSFX);
        }
    }
}
