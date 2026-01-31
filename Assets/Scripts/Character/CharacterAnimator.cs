using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    [SerializeField] FacingDirection currentFacingDirection;

    // Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }
    public bool IsJumping { get; set; }

    // States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;
    // References
    [SerializeField] SpriteRenderer spriteRenderer;

    private bool isScaring;
    [SerializeField] private GameObject scareGameObject;
    private void Start()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        SetfacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }
    private void Update()
    {
        if (isScaring)
        {
            spriteRenderer.sprite = null;
            return;
        }

        var prevAnim = currentAnim;

        // Handle horizontal facing (left/right) - takes priority for top-down mouse aiming
        if (MoveX == 1)
        {
            currentAnim = walkRightAnim;
            currentFacingDirection = FacingDirection.Right;
        }
        else if (MoveX == -1)
        {
            currentAnim = walkLeftAnim;
            currentFacingDirection = FacingDirection.Left;
        }
        else if (MoveY == 1)
        {
            currentAnim = walkUpAnim;
            currentFacingDirection = FacingDirection.Up;
        }
        else if (MoveY == -1)
        {
            currentAnim = walkDownAnim;
            currentFacingDirection = FacingDirection.Down;
        }

        if (currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
            currentAnim.Start();

        if (IsJumping)
            spriteRenderer.sprite = currentAnim.Frames[currentAnim.Frames.Count - 1];
        else if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPreviouslyMoving = IsMoving;
    }

    public void SetfacingDirection(FacingDirection dir)
    {
        currentFacingDirection = dir;

        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
    }
    public FacingDirection GetCurrentFacingDirection() => currentFacingDirection;

    public FacingDirection DefaultDirection
    {
        get => defaultDirection;
    }

    public void SetIsScaring(bool isScaring)
    {
        this.isScaring = isScaring;
        scareGameObject.SetActive(isScaring);
    }
}

public enum FacingDirection { Up, Down, Left, Right }
