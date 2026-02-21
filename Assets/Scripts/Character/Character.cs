using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]

public class Character : MonoBehaviour
{

    [SerializeField] MaskDisplay maskDisplay;
    public float moveSpeed;

    [Header("Mask Bob")]
    [SerializeField] float maskBobFrequency = 12f;
    [SerializeField] float maskBobAmplitude = 0.08f;
    [SerializeField] float maskBobDelay = 0.05f;
    float maskBobTimer;
    float delayedBobOffset;
    Vector3 maskBasePosition;

    [Header("Walk Wobble")]
    [SerializeField] float wobbleFrequency = 24f;
    [SerializeField] float wobbleAmplitude = 36f;
    [SerializeField] float bodyBobAmplitude = 0.05f;
    [SerializeField] float bodyBobPhaseOffset = 1.5f;
    Transform spriteTransform;
    Vector3 spriteBasePosition;
    float wobbleTimer;

    [Header("Walking Sound (Continuous)")]
    [SerializeField] AudioClip walkingLoopSound;
    [SerializeField] AudioClip spiritWalkingLoopSound;
    [SerializeField] float walkingSoundVolume = 0.7f; // 30% quieter by default
    [SerializeField] float walkingSoundPitch = 1.3f; // Speed up the loop
    [SerializeField] float enemyWalkingMaxDistance = 10f; // Distance at which enemy walking is silent
    private AudioSource walkingAudioSource;
    private bool isWalkingSoundPlaying;

    Vector3 lastFramePosition;
    float currentSpeed;

    public bool temporarilyForbidMovement = false;
    public bool IsMoving { get; private set; }

    public bool IsPlayerCharacter;
    public bool IsNPCCharacter => !IsPlayerCharacter;

    public MaskType CurrentMask
    {
        get
        {
            return maskDisplay.currentMask;
        }
        set
        {
            maskDisplay.SwitchMask(value);
        }
    }


    public CharacterAnimator characterAnimator;

    CharacterAnimator animator;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        //SetPositionAndSnapToTile(transform.position); // Snap do centra tilu
        CurrentMask = MaskType.Default;
        moveSpeed *= 0.5f; // Slow down movement by 2x

        // Initialize bob/wobble transforms
        if (maskDisplay != null)
        {
            maskBasePosition = maskDisplay.transform.localPosition;
        }
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteTransform = spriteRenderer != null ? spriteRenderer.transform : transform;
        spriteBasePosition = spriteTransform.localPosition;
        lastFramePosition = transform.position;

        // Create dedicated audio source for walking loop
        var walkingAudioGO = new GameObject("WalkingSound");
        walkingAudioGO.transform.SetParent(transform);
        walkingAudioGO.transform.localPosition = Vector3.zero;
        walkingAudioSource = walkingAudioGO.AddComponent<AudioSource>();
        walkingAudioSource.loop = true;
        walkingAudioSource.playOnAwake = false;
        walkingAudioSource.spatialBlend = 0f; // 2D sound
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;

        transform.position = pos;
    }


    public void HandleUpdate()
    {
        if (animator != null)
        {
            animator.IsMoving = IsMoving && !temporarilyForbidMovement;
        }
        if (temporarilyForbidMovement)
            return;

        // Calculate speed from actual position change
        Vector3 delta = transform.position - lastFramePosition;
        float targetSpeed = Time.deltaTime > 0 ? delta.magnitude / Time.deltaTime : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);
        lastFramePosition = transform.position;

        UpdateBobAndWobble();
        UpdateWalkingSound();
    }

    void UpdateWalkingSound()
    {
        if (walkingAudioSource == null) return;

        bool shouldPlay = currentSpeed > 0.1f;

        // Calculate volume with distance attenuation for enemies
        float volume = walkingSoundVolume;
        if (!IsPlayerCharacter && PlayerController.Instance != null)
        {
            float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
            float distanceAttenuation = 1f - Mathf.Clamp01(distance / enemyWalkingMaxDistance);
            volume *= distanceAttenuation;
        }

        // Get the right clip based on spirit realm state
        bool inSpiritRealm = AstralPlane.Instance?.isActiveAndEnabled ?? false;
        AudioClip targetClip = inSpiritRealm ? spiritWalkingLoopSound : walkingLoopSound;

        // Start/stop walking sound
        if (shouldPlay && volume > 0.01f)
        {
            // Switch clip if needed
            if (walkingAudioSource.clip != targetClip && targetClip != null)
            {
                walkingAudioSource.clip = targetClip;
                if (isWalkingSoundPlaying)
                    walkingAudioSource.Play();
            }

            if (!isWalkingSoundPlaying && targetClip != null)
            {
                walkingAudioSource.clip = targetClip;
                walkingAudioSource.Play();
                isWalkingSoundPlaying = true;
            }

            walkingAudioSource.volume = volume;
            walkingAudioSource.pitch = walkingSoundPitch;
        }
        else if (isWalkingSoundPlaying)
        {
            walkingAudioSource.Stop();
            isWalkingSoundPlaying = false;
        }
    }

    void UpdateBobAndWobble()
    {
        // Mask bob with delay
        if (maskDisplay != null)
        {
            Transform maskTransform = maskDisplay.transform;
            if (currentSpeed > 0.1f)
            {
                maskBobTimer += Time.deltaTime * maskBobFrequency;
                float targetBobOffset = Mathf.Sin(maskBobTimer) * maskBobAmplitude;
                float delaySpeed = maskBobDelay > 0 ? 1f / maskBobDelay : 100f;
                delayedBobOffset = Mathf.Lerp(delayedBobOffset, targetBobOffset, Time.deltaTime * delaySpeed);
                maskTransform.localPosition = maskBasePosition + new Vector3(0, delayedBobOffset, 0);
            }
            else
            {
                delayedBobOffset = Mathf.Lerp(delayedBobOffset, 0, Time.deltaTime * 10f);
                maskTransform.localPosition = Vector3.Lerp(maskTransform.localPosition, maskBasePosition, Time.deltaTime * 10f);
                maskBobTimer = 0;
            }
        }

        // Walk wobble - sway rotation + vertical bob
        if (spriteTransform != null)
        {
            if (currentSpeed > 0.1f)
            {
                wobbleTimer += Time.deltaTime * wobbleFrequency;
                float wobbleAngle = Mathf.Sin(wobbleTimer) * wobbleAmplitude;
                spriteTransform.localRotation = Quaternion.Euler(0, 0, wobbleAngle);
                float bobOffset = Mathf.Sin(wobbleTimer + bodyBobPhaseOffset) * bodyBobAmplitude;
                spriteTransform.localPosition = spriteBasePosition + new Vector3(0, bobOffset, 0);
            }
            else
            {
                spriteTransform.localRotation = Quaternion.Slerp(spriteTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
                spriteTransform.localPosition = Vector3.Lerp(spriteTransform.localPosition, spriteBasePosition, Time.deltaTime * 10f);
                wobbleTimer = 0;
            }
        }
    }

    public CharacterAnimator Animator
    {
        get => animator;
    }

    public MaskDisplay MaskDisplay => maskDisplay;
}
