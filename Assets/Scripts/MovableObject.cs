using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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

    [Header("Player Proximity")]
    [SerializeField] float playerActivationDistance = 1.5f;

    Rigidbody2D rb;
    AudioSource audioSource;
    float targetVolume;

    public float originalMass;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        originalMass = rb.mass;
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


        // Only enable physics when player is close enough to push
        bool playerClose = false;
        var player = PlayerController.Instance?.controllingCharacter;
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            playerClose = dist <= playerActivationDistance;
        }

        if (PlayerController.Instance.CanPlayerPushObjects && Pushable && playerClose)
            rb.mass = originalMass;
        else
            // keep dynamic so it sends collisons to PressurePlate
            // but make it not pushable by physics
            rb.mass = 100000000f;


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
