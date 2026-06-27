using UnityEngine;
using Zenject;

public class MainMenuController : MonoBehaviour
{
    private AudioManager _audioManager;
    private AudioLibrary _audioLibrary;

    [Inject]
    public void Construct(AudioManager audioManager, AudioLibrary audioLibrary)
    {
        _audioManager = audioManager;
        _audioLibrary = audioLibrary;
    }

    private void Start()
    {
        _audioManager.CrossfadeSceneAmbience(_audioLibrary.MainMenuAmbience);
    }
}