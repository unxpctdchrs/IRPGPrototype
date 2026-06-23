using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class OldMan : MonoBehaviour
{
    [Header("Footstep Sounds")]
    [SerializeField] private AudioResource[] _footstepClips;
    [SerializeField] private float _volume = 0.6f;

    private AudioManager _audioManager;

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public void PlayFootstep()
    {
        if (_audioManager == null || _footstepClips.Length == 0) return;
        int randomIndex = Random.Range(0, _footstepClips.Length);
        AudioResource selectedClip = _footstepClips[randomIndex];
        _audioManager.PlaySFXAtPosition(selectedClip, transform.position, _volume);
    }
}
