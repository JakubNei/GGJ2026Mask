using System.Collections;
using UnityEngine;

/// <summary>
/// Attach to any GameObject in a scene to play music when the scene loads.
/// </summary>
public class LevelMusic : MonoBehaviour
{
    [SerializeField] AudioClip musicClip;
    [SerializeField] bool loop = true;
    [SerializeField] bool fadeIn = true;
    [SerializeField] [Range(0f, 1f)] float volume = 0.6f;
    [SerializeField] float delay = 2f;

    void Start()
    {
        if (musicClip != null && AudioManager.i != null)
        {
            StartCoroutine(PlayMusicWithDelay());
        }
    }

    IEnumerator PlayMusicWithDelay()
    {
        yield return new WaitForSeconds(delay);
        AudioManager.i.SetMusicVolume(volume);
        AudioManager.i.PlayMusic(musicClip, loop, fadeIn);
    }
}
