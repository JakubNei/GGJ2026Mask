using System.Collections.Generic;
using UnityEngine;

public class WalkAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> walkSpritesDown;  // front_ sprites (left/right/down)
    [SerializeField] private List<Sprite> walkSpritesUp;    // back_ sprites (up)
    [SerializeField] private Sprite idleSpriteDown;         // idle when facing down (optional, uses walkSpritesDown[0] if not set)
    [SerializeField] private Sprite idleSpriteUp;           // idle when facing up (optional, uses walkSpritesUp[0] if not set)
    [SerializeField] private float frameRate = 0.1f;
    [SerializeField] private float movementLingerTime = 0.1f;

    private int currentFrame = 0;
    private float animTimer = 0f;
    private Vector3 lastPosition;
    private bool isAnimating = false;
    private float timeSinceLastMovement = 0f;
    private FacingDirection currentDirection = FacingDirection.Down;

    public FacingDirection CurrentDirection => currentDirection;

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

        UpdateFacingDirection();

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
    }

    private void UpdateFacingDirection()
    {
        if (Camera.main == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = transform.position;
        Vector2 dir = new Vector2(mouseWorldPos.x - playerPos.x, mouseWorldPos.y - playerPos.y);

        if (dir.sqrMagnitude < 0.001f) return;

        // Calculate angle from player to mouse (0 = right, 90 = up, 180/-180 = left, -90 = down)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Up = top 120 degrees (from 30 to 150) - uses back_ sprites
        // Down = bottom 240 degrees - uses front_ sprites
        if (angle > 30f && angle < 150f)
            currentDirection = FacingDirection.Up;
        else
            currentDirection = FacingDirection.Down;
    }
}
