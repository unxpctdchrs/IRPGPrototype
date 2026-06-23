using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [Header("Player")]
    public AudioResource StepWood;
    public AudioResource StepConcrete;
}