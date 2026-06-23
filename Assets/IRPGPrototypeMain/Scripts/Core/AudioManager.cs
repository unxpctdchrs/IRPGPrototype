using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Zenject;

public class AudioManager : MonoBehaviour, IInitializable
{
    [Header("Settings")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup ambienceMixerGroup;
    [SerializeField] private int poolSize = 10;

    private List<AudioSource> sfxSources;
    private AudioSource musicSource;
    private List<AudioSource> ambienceSources;
    private int maxAmbienceLayers = 3;

    public void Initialize()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        sfxSources = new List<AudioSource>();
        ambienceSources = new List<AudioSource>();

        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.SetParent(transform);
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicMixerGroup;
        musicSource.loop = true;

        for (int i = 0; i < maxAmbienceLayers; i++)
        {
            GameObject ambObj = new GameObject($"AmbienceSource_{i}");
            ambObj.transform.SetParent(transform);
            AudioSource ambSource = ambObj.AddComponent<AudioSource>();
            ambSource.outputAudioMixerGroup = ambienceMixerGroup;
            ambSource.loop = true;
            ambienceSources.Add(ambSource);
        }

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewSource();
        }
    }

    private AudioSource CreateNewSource()
    {
        GameObject obj = new GameObject($"SFX_Source_{sfxSources.Count}");
        obj.transform.SetParent(transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.playOnAwake = false;
        sfxSources.Add(source);
        return source;
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying) return source;
        }
        return CreateNewSource();
    }

    public void PlaySFX(AudioResource container, float volume = 1f)
    {
        AudioSource source = GetAvailableSource();
        source.spatialBlend = 0f; 
        source.resource = container;
        source.volume = volume;
        source.Play();
    }

    public void PlaySFXAtPosition(AudioResource container, Vector3 position, float volume = 1f)
    {
        AudioSource source = GetAvailableSource();
        source.transform.position = position;
        source.spatialBlend = 1f;
        source.minDistance = 2f;
        source.maxDistance = 20f;
        source.resource = container;
        source.volume = volume;
        source.loop = false;
        source.Play();
    }

    public AudioSource PlaySFXAtPositionLoop(AudioResource container, Vector3 position, float volume = 1f)
    {
        AudioSource source = GetAvailableSource();
        source.transform.position = position;
        source.spatialBlend = 1f;
        source.minDistance = 2f;
        source.maxDistance = 20f;
        source.resource = container;
        source.volume = volume;
        source.loop = true;
        source.Play();
        return source;
    }

    public void StopLoopingSFX(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.loop = false;
            source.resource = null;
        }
    }
    
    public void PlayMusic(AudioResource musicContainer)
    {
        if (musicSource.resource == musicContainer && musicSource.isPlaying) return;
        
        musicSource.Stop();
        musicSource.resource = musicContainer;
        musicSource.Play();
    }

    public void PlayAmbience(AudioResource clip, float volume = 1f)
    {
        foreach (var source in ambienceSources)
        {
            if (source.resource == clip && source.isPlaying)
            {
                source.volume = volume; 
                return;
            }
        }

        foreach (var source in ambienceSources)
        {
            if (!source.isPlaying)
            {
                source.resource = clip;
                source.volume = volume;
                source.Play();
                return;
            }
        }

        Debug.LogWarning("All ambience channels are full! Increase maxAmbienceLayers.");
    }
    
    public void StopAmbience(AudioResource clip)
    {
        foreach (var source in ambienceSources)
        {
            if (source.resource == clip)
            {
                source.Stop();
                source.resource = null;
                return;
            }
        }
    }
}