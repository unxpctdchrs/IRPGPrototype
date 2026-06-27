using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    public AudioResource ShedAmbience;
    public AudioResource WorldAmbience;
    public AudioResource MainMenuAmbience;
    public AudioResource StepWood;
    public AudioResource StepConcrete;
    public AudioResource AmbushSFX;
    public AudioResource BattleMusic;
}