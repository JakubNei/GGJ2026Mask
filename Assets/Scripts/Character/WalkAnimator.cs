using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection { Up, Down, Left, Right }

public class WalkAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> walkSpritesDown;  // front_ sprites (left/right/down)
    [SerializeField] private List<Sprite> walkSpritesUp;    // back_ sprites (up)
    [SerializeField] private Sprite idleSpriteDown;         // idle when facing down (optional, uses walkSpritesDown[0] if not set)
    [SerializeField] private Sprite idleSpriteUp;           // idle when facing up (optional, uses walkSpritesUp[0] if not set)
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private float movementLingerTime = 0.1f;

    public int currentFrame = 0;
    public float animTimer = 0f;
    public Vector3 lastPosition;
    public bool isAnimating = false;
    public float timeSinceLastMovement = 0f;
    public FacingDirection currentDirection = FacingDirection.Down;

    public static FacingDirection GetFacingFromDirection(Vector2 dir)
    {
        if (dir.magnitude < 0.01f)
            return FacingDirection.Down; // defaultGet

        // dir = dir.normalized;
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // if (angle < 0) angle += 360f;

        // if (angle >= 45f && angle < 135f)
        //     return FacingDirection.Up;
        // else if (angle >= 135f && angle < 225f)
        //     return FacingDirection.Left;
        // else if (angle >= 225f && angle < 315f)
        //     return FacingDirection.Down;
        // else
        //     return FacingDirection.Right;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
                return FacingDirection.Right;
            else
                return FacingDirection.Left;
        }
        else
        {
            if (dir.y > 0)
                return FacingDirection.Up;
            else
                return FacingDirection.Down;
        }
    }

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

        lastPosition = transform.position;
    }

    private void Update()
    {
        if (spriteRenderer == null)
            return;

        List<Sprite> currentSprites = currentDirection == FacingDirection.Up ? walkSpritesUp : walkSpritesDown;

        if (currentSprites == null || currentSprites.Count == 0)
            return;

        // Detect movement by position change
        Vector3 currentPos = transform.position;
        float sqrDist = (currentPos - lastPosition).sqrMagnitude;
        bool movedThisFrame = sqrDist > 0.0001f;
        lastPosition = currentPos;

        if (movedThisFrame)
            timeSinceLastMovement = 0f;
        else
            timeSinceLastMovement += Time.deltaTime;

        bool shouldAnimate = timeSinceLastMovement < movementLingerTime;

        if (shouldAnimate)
        {
            // Animating - cycle through frames
            if (!isAnimating)
            {
                isAnimating = true;
                currentFrame = 0;
                animTimer = 0f;
            }

            animTimer += Time.deltaTime;
            if (animTimer >= frameRate)
            {
                animTimer = 0f;
                currentFrame = (currentFrame + 1) % currentSprites.Count;
            }
            spriteRenderer.sprite = currentSprites[currentFrame];
        }
        else
        {
            // Idle - show idle sprite or first frame of current direction
            isAnimating = false;
            Sprite idleSprite = currentDirection == FacingDirection.Up ? idleSpriteUp : idleSpriteDown;
            if (idleSprite != null)
                spriteRenderer.sprite = idleSprite;
            else if (currentSprites.Count > 0)
                spriteRenderer.sprite = currentSprites[0];
        }

        spriteRenderer.flipX = currentDirection == FacingDirection.Up ? false : currentDirection == FacingDirection.Left;
    }

}
