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
    private Coroutine _ambienceCrossfadeRoutine;

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
        Debug.Log($"Playing {container.name} on source {source.name}. Is playing: {source.isPlaying}, Volume: {volume}");
        source.spatialBlend = 0f; 
        source.reverbZoneMix = 1f;
        source.bypassEffects = false;
        source.resource = container;
        source.volume = volume;
        source.pitch = 1f;
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
    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.resource = null;
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

    public void CrossfadeSceneAmbience(AudioResource newAmbience, float fadeDuration = 2.0f)
    {
        if (newAmbience == null) return;
        if (_ambienceCrossfadeRoutine != null)
        {
            StopCoroutine(_ambienceCrossfadeRoutine);
        }
        _ambienceCrossfadeRoutine = StartCoroutine(CrossfadeAmbienceRoutine(newAmbience, fadeDuration));
    }

    private System.Collections.IEnumerator CrossfadeAmbienceRoutine(AudioResource newAmbience, float duration)
    {
        AudioSource targetSource = null;
        Dictionary<AudioSource, float> startVolumes = new Dictionary<AudioSource, float>();

        foreach (var source in ambienceSources)
        {
            if (source.isPlaying) startVolumes[source] = source.volume;
            if (source.resource == newAmbience)
            {
                targetSource = source;
            }
        }

        if (targetSource == null)
        {
            foreach (var source in ambienceSources)
            {
                if (!source.isPlaying)
                {
                    targetSource = source;
                    targetSource.resource = newAmbience;
                    targetSource.volume = 0f;
                    targetSource.Play();
                    break;
                }
            }
        }

        if (targetSource == null)
        {
            Debug.LogWarning("[AudioManager] All ambience channels full! Cannot crossfade.");
            yield break;
        }

        if (!startVolumes.ContainsKey(targetSource)) startVolumes[targetSource] = targetSource.volume;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = timer / duration;

            foreach (var source in ambienceSources)
            {
                if (source == targetSource)
                {
                    source.volume = Mathf.Lerp(startVolumes[source], 1f, percent);
                }
                else if (source.isPlaying)
                {
                    source.volume = Mathf.Lerp(startVolumes[source], 0f, percent);
                }
            }
            yield return null;
        }

        foreach (var source in ambienceSources)
        {
            if (source == targetSource)
            {
                source.volume = 1f;
            }
            else if (source.isPlaying)
            {
                source.volume = 0f;
                source.Stop();
                source.resource = null;
            }
        }
    }
}