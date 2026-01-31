using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class MovableObject : MonoBehaviour, IInteractable
{
    public bool Pullable = false;
    public bool Pushable = true;
    //public bool ForbidDiagonalMovement = true;

    [Header("Audio")]
    [SerializeField] AudioClip pushSound;
    [SerializeField] float pushVelocityThreshold = 0.1f;
    [SerializeField] float maxVolume = 0.4f;
    [SerializeField] float fadeDuration = 0.5f;

    Rigidbody2D rb;
    AudioSource audioSource;
    float targetVolume;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = pushSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }
    public bool CanInteract()
    {
        return Pullable && PlayerController.Instance.CanPlayerPullObjects;
    }

    public void UpdateWhileInteracting()
    {
        if (!PlayerController.Instance.CanPlayerPullObjects || !Pullable)
            return;

        // if (rb.bodyType == RigidbodyType2D.Dynamic)
        //     rb.MovePosition(rb.position + (Vector2)PlayerController.Instance.lastCharacterDeltaMovement);
        // else
        transform.position += PlayerController.Instance.lastCharacterDeltaMovement;
    }

    void Update()
    {
        // Does not do what I want, I was hoping it would make the movmenet more arcadish snappy alix aligned
        // if (ForbidDiagonalMovement)
        // {
        //     if (rb.linearVelocity.magnitude > 0)
        //     {
        //         if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(rb.linearVelocity.y))
        //         {
        //             rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        //         }
        //         else
        //         {
        //             rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        //         }
        //     }
        //     else
        //     {
        //         rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //     }
        // }

        rb.bodyType =
                PlayerController.Instance.CanPlayerPushObjects && Pushable ?
                RigidbodyType2D.Dynamic :
                RigidbodyType2D.Static;

        // Play looping push sound when being pushed
        bool shouldPlaySound = rb.bodyType == RigidbodyType2D.Dynamic &&
                               rb.linearVelocity.magnitude > pushVelocityThreshold &&
                               pushSound != null;

        targetVolume = shouldPlaySound ? maxVolume : 0f;

        // Fade volume
        if (audioSource.volume != targetVolume)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, maxVolume / fadeDuration * Time.deltaTime);
        }

        // Start/stop based on volume
        if (audioSource.volume > 0.01f && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (audioSource.volume <= 0.01f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public bool CanPickUp()
    {
        return false;
    }
}
