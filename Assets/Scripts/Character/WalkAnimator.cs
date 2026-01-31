using System.Collections.Generic;
using UnityEngine;

public class WalkAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> walkSprites;
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private float movementLingerTime = 0.1f; // How long to keep animating after movement stops


    private Sprite idleSprite;
    private int currentFrame = 0;
    private float animTimer = 0f;
    private Vector3 lastPosition;
    private bool isAnimating = false;
    private float timeSinceLastMovement = 0f;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError($"[WalkAnimator] No SpriteRenderer found on '{gameObject.name}'!");
            enabled = false;
            return;
        }

        idleSprite = spriteRenderer.sprite;
        lastPosition = transform.position;

        int validSprites = 0;
        if (walkSprites != null)
        {
            foreach (var s in walkSprites)
                if (s != null) validSprites++;
        }

        Debug.Log($"[WalkAnimator] Init: {validSprites} sprites, renderer='{spriteRenderer.gameObject.name}'");

        if (validSprites == 0)
        {
            Debug.LogError($"[WalkAnimator] No walk sprites assigned!");
            enabled = false;
        }
    }

    private void Update()
    {
        if (spriteRenderer == null || walkSprites == null || walkSprites.Count == 0)
            return;

        // Detect movement by position change
        Vector3 currentPos = transform.position;
        float sqrDist = (currentPos - lastPosition).sqrMagnitude;
        bool movedThisFrame = sqrDist > 0.0001f;
        lastPosition = currentPos;

        // Track time since last actual movement
        if (movedThisFrame)
        {
            timeSinceLastMovement = 0f;
        }
        else
        {
            timeSinceLastMovement += Time.deltaTime;
        }

        // Consider "moving" if we moved recently (within linger time)
        bool shouldAnimate = timeSinceLastMovement < movementLingerTime;

        // Started animating
        if (shouldAnimate && !isAnimating)
        {
            Debug.Log($"[WalkAnimator] Started walking");
            isAnimating = true;
            currentFrame = 0;
            animTimer = 0f;
            spriteRenderer.sprite = walkSprites[currentFrame];
        }

        // Stopped animating
        if (!shouldAnimate && isAnimating)
        {
            Debug.Log($"[WalkAnimator] Stopped walking");
            isAnimating = false;
            spriteRenderer.sprite = idleSprite;
            return;
        }

        // Animate while moving
        if (isAnimating)
        {
            animTimer += Time.deltaTime;
            if (animTimer >= frameRate)
            {
                animTimer = 0f;
                currentFrame = (currentFrame + 1) % walkSprites.Count;
                spriteRenderer.sprite = walkSprites[currentFrame];
            }
        }
    }
}
