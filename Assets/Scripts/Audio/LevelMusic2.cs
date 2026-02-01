using DG.Tweening;
using UnityEngine;

/// <summary>
/// For levels with astral plane - plays both tracks simultaneously and crossfades volumes.
/// Music continues from where it was when switching back (Steam-like behavior).
/// </summary>
public class LevelMusic2 : MonoBehaviour
{
    [SerializeField] AudioClip normalMusic;
    [SerializeField] AudioClip astralMusic;
    [SerializeField] bool loop = true;
    [SerializeField] [Range(0f, 1f)] float volume = 0.6f;
    [SerializeField] float fadeDuration = 0.75f;
    [SerializeField] float startDelay = 2f;

    AudioSource normalSource;
    AudioSource astralSource;
    bool wasAstral;
    bool started;
    bool astralStarted;

    void Start()
    {
        // Create two audio sources
        normalSource = gameObject.AddComponent<AudioSource>();
        astralSource = gameObject.AddComponent<AudioSource>();

        SetupSource(normalSource, normalMusic);
        SetupSource(astralSource, astralMusic);

        wasAstral = IsAstralPlaneActive();

        Invoke(nameof(StartMusic), startDelay);
    }

    void SetupSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.loop = loop;
        source.playOnAwake = false;
        source.volume = 0f;
    }

    void StartMusic()
    {
        // Start only normal music at the beginning
        if (normalMusic != null)
        {
            normalSource.Play();
            normalSource.DOFade(volume, fadeDuration);
        }

        started = true;
    }

    void Update()
    {
        if (!started) return;

        bool isAstral = IsAstralPlaneActive();
        if (isAstral != wasAstral)
        {
            wasAstral = isAstral;
            Crossfade();
        }
    }

    void Crossfade()
    {
        if (wasAstral)
        {
            // First time entering astral - start the astral track
            if (!astralStarted && astralMusic != null)
            {
                astralSource.Play();
                astralStarted = true;
            }

            normalSource.DOFade(0f, fadeDuration);
            astralSource.DOFade(volume, fadeDuration);
        }
        else
        {
            astralSource.DOFade(0f, fadeDuration);
            normalSource.DOFade(volume, fadeDuration);
        }
    }

    bool IsAstralPlaneActive()
    {
        var astral = AstralPlane.Instance;
        return astral != null && astral.gameObject.activeSelf;
    }

    void OnDestroy()
    {
        // Fade out both
        normalSource?.DOFade(0f, fadeDuration);
        astralSource?.DOFade(0f, fadeDuration);
    }
}
