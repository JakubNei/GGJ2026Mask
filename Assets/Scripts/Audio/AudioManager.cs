using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] int sfxPoolSize = 8;

    [Header("Audio Mixer (Optional)")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup musicGroup;
    [SerializeField] AudioMixerGroup sfxGroup;

    [Header("Settings")]
    [SerializeField] float fadeDuration = 0.75f;
    [SerializeField] float defaultPitchVariation = 0.05f;

    [Header("Sound Database")]
    [SerializeField] List<AudioData> sfxList;

    // Runtime
    AudioSource[] sfxPool;
    int nextSfxIndex;
    AudioClip currMusic;
    float originalMusicVol;
    Dictionary<AudioId, AudioData> sfxLookup;

    // Volume settings (0-1)
    float masterVolume = 1f;
    float musicVolume = 1f;
    float sfxVolume = 1f;

    public static AudioManager i { get; private set; }

    private void Awake()
    {
        i = this;
        InitializeSfxPool();
    }

    private void Start()
    {
        originalMusicVol = musicPlayer.volume;
        sfxLookup = sfxList?.ToDictionary(x => x.id) ?? new Dictionary<AudioId, AudioData>();

        // Assign mixer groups if available
        if (musicGroup != null)
            musicPlayer.outputAudioMixerGroup = musicGroup;
    }

    void InitializeSfxPool()
    {
        sfxPool = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var go = new GameObject($"SFX_Source_{i}");
            go.transform.SetParent(transform);
            sfxPool[i] = go.AddComponent<AudioSource>();
            sfxPool[i].playOnAwake = false;

            if (sfxGroup != null)
                sfxPool[i].outputAudioMixerGroup = sfxGroup;
        }
    }

    AudioSource GetNextSfxSource()
    {
        var source = sfxPool[nextSfxIndex];
        nextSfxIndex = (nextSfxIndex + 1) % sfxPoolSize;
        return source;
    }

    #region SFX Playback

    /// <summary>
    /// Play sound effect (2D, no position)
    /// </summary>
    public void PlaySfx(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null) return;

        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }

        var source = GetNextSfxSource();
        source.spatialBlend = 0f; // 2D
        source.pitch = 1f;
        source.volume = sfxVolume * masterVolume;
        source.PlayOneShot(clip);
    }

    /// <summary>
    /// Play sound effect by AudioId
    /// </summary>
    public void PlaySfx(AudioId audioId, bool pauseMusic = false)
    {
        if (!sfxLookup.ContainsKey(audioId))
        {
            Debug.LogWarning($"AudioManager: AudioId '{audioId}' not found in sfxList");
            return;
        }

        var audioData = sfxLookup[audioId];
        PlaySfx(audioData.clip, pauseMusic);
    }

    /// <summary>
    /// Play sound effect with pitch variation (good for footsteps, impacts)
    /// </summary>
    public void PlaySfxWithVariation(AudioClip clip, float pitchVariation = 0f)
    {
        if (clip == null) return;

        float variation = pitchVariation > 0 ? pitchVariation : defaultPitchVariation;

        var source = GetNextSfxSource();
        source.spatialBlend = 0f;
        source.pitch = 1f + UnityEngine.Random.Range(-variation, variation);
        source.volume = sfxVolume * masterVolume;
        source.PlayOneShot(clip);
    }

    /// <summary>
    /// Play sound effect with pitch variation by AudioId
    /// </summary>
    public void PlaySfxWithVariation(AudioId audioId, float pitchVariation = 0f)
    {
        if (!sfxLookup.ContainsKey(audioId)) return;
        PlaySfxWithVariation(sfxLookup[audioId].clip, pitchVariation);
    }

    /// <summary>
    /// Play sound at world position (3D spatial audio with distance attenuation)
    /// </summary>
    public void PlaySfxAt(AudioClip clip, Vector3 position, float minDistance = 1f, float maxDistance = 15f)
    {
        if (clip == null) return;

        var source = GetNextSfxSource();
        source.transform.position = position;
        source.spatialBlend = 1f; // Full 3D
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.pitch = 1f;
        source.volume = sfxVolume * masterVolume;
        source.PlayOneShot(clip);
    }

    /// <summary>
    /// Play random clip from array (good for variety)
    /// </summary>
    public void PlayRandomSfx(AudioClip[] clips, float pitchVariation = 0f)
    {
        if (clips == null || clips.Length == 0) return;
        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (pitchVariation > 0)
            PlaySfxWithVariation(clip, pitchVariation);
        else
            PlaySfx(clip);
    }

    /// <summary>
    /// Play random clip from array with custom volume multiplier and pitch variation
    /// </summary>
    public void PlayRandomSfxWithVolume(AudioClip[] clips, float volumeMultiplier, float pitchVariation = 0f)
    {
        if (clips == null || clips.Length == 0) return;
        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        PlaySfxWithVolume(clip, volumeMultiplier, pitchVariation);
    }

    /// <summary>
    /// Play sound effect with custom volume multiplier and pitch variation
    /// </summary>
    public void PlaySfxWithVolume(AudioClip clip, float volumeMultiplier, float pitchVariation = 0f)
    {
        if (clip == null) return;

        float variation = pitchVariation > 0 ? pitchVariation : defaultPitchVariation;

        var source = GetNextSfxSource();
        source.spatialBlend = 0f;
        source.pitch = 1f + UnityEngine.Random.Range(-variation, variation);
        source.volume = sfxVolume * masterVolume * Mathf.Clamp01(volumeMultiplier);
        source.PlayOneShot(clip);
    }

    /// <summary>
    /// Play sound effect by AudioId with custom volume multiplier and pitch variation
    /// </summary>
    public void PlaySfxWithVolume(AudioId audioId, float volumeMultiplier, float pitchVariation = 0f)
    {
        if (!sfxLookup.ContainsKey(audioId)) return;
        PlaySfxWithVolume(sfxLookup[audioId].clip, volumeMultiplier, pitchVariation);
    }

    #endregion

    #region Music Playback

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false)
    {
        if (clip == null || clip == currMusic) return;

        currMusic = clip;
        StartCoroutine(PlayMusicAsync(clip, loop, fade));
    }

    public void StopMusic(bool fade = true)
    {
        if (fade)
            StartCoroutine(StopMusicAsync());
        else
        {
            musicPlayer.Stop();
            currMusic = null;
        }
    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    {
        if (fade && musicPlayer.isPlaying)
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();

        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.volume = fade ? 0 : originalMusicVol * musicVolume * masterVolume;
        musicPlayer.Play();

        if (fade)
            yield return musicPlayer.DOFade(originalMusicVol * musicVolume * masterVolume, fadeDuration).WaitForCompletion();
    }

    IEnumerator StopMusicAsync()
    {
        yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        musicPlayer.Stop();
        currMusic = null;
    }

    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalMusicVol * musicVolume * masterVolume, fadeDuration);
    }

    #endregion

    #region Volume Control

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateMusicVolume();

        if (audioMixer != null)
            audioMixer.SetFloat("MasterVolume", VolumeToDecibels(masterVolume));
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateMusicVolume();

        if (audioMixer != null)
            audioMixer.SetFloat("MusicVolume", VolumeToDecibels(musicVolume));
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", VolumeToDecibels(sfxVolume));
    }

    void UpdateMusicVolume()
    {
        if (musicPlayer != null && musicPlayer.isPlaying)
            musicPlayer.volume = originalMusicVol * musicVolume * masterVolume;
    }

    float VolumeToDecibels(float volume)
    {
        // Convert 0-1 to decibels (-80 to 0)
        return volume > 0.0001f ? Mathf.Log10(volume) * 20f : -80f;
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSfxVolume() => sfxVolume;

    #endregion

    #region Scene Transition Fades

    float savedMusicVolume;
    float savedSfxVolume;

    /// <summary>
    /// Fade out all audio (music and SFX) for scene transitions
    /// </summary>
    public void FadeOutAll(float duration)
    {
        savedMusicVolume = musicPlayer.volume;
        savedSfxVolume = sfxVolume;

        // Fade music
        if (musicPlayer.isPlaying)
            musicPlayer.DOFade(0, duration);

        // Fade SFX pool
        foreach (var source in sfxPool)
        {
            if (source != null && source.isPlaying)
                source.DOFade(0, duration);
        }
    }

    /// <summary>
    /// Fade in all audio after scene transition
    /// </summary>
    public void FadeInAll(float duration)
    {
        // Restore music volume
        if (musicPlayer.isPlaying)
            musicPlayer.DOFade(savedMusicVolume > 0 ? savedMusicVolume : originalMusicVol * musicVolume * masterVolume, duration);

        // SFX will play at normal volume for new sounds
        sfxVolume = savedSfxVolume > 0 ? savedSfxVolume : 1f;
    }

    #endregion
}

/// <summary>
/// Sound IDs for centrally managed audio clips
/// </summary>
public enum AudioId
{
    // UI
    UISelect,
    MenuClick,
    MenuSelect,
    MenuHover,

    // Music
    MusicMenu,
    MusicSpiritWorld,

    // Transitions
    EnterRealWorld,
    ExitRealWorld,
    EnterSpiritRealm,
    ExitSpiritRealm,

    // Player
    WalkRealWorld,
    WalkSpiritRealm,
    SwitchMask,
    PickupMask,

    // Tough Mask
    ToughPushBlock,
    ToughPushCart,
    ToughPressureTile,

    // Ninja Mask
    NinjaShootBlowgun,
    NinjaShootStick,
    NinjaHitButton,
    NinjaHitVase,
    NinjaHiddenTile,

    // World
    DoorOpen,

    // Legacy (keep for compatibility)
    HouseBurning,
    ClosingMenu,
    OpenMenu,
    HoveringOverMenu,
    ButtonClick,
    GrandmaScared,
    ScissorsCutting
}

[Serializable]
public class AudioData
{
    public AudioId id;
    public AudioClip clip;
}
