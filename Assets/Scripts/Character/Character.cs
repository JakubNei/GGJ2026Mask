using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]

public class Character : MonoBehaviour
{

    [SerializeField] AudioClip fallSfx;
    [SerializeField] Sprite poisonedSprite;
    [SerializeField] GameObject defaultMask;
    [SerializeField] GameObject toughMask;
    [SerializeField] GameObject ninjaMask;
    [SerializeField] GameObject shamanMask;
    public float moveSpeed;

    [SerializeField] public string Name;
    public bool IsMoving { get; private set; }

    public float OffsetY { get; private set; } = 0.5f;

    public bool HasFallen { get; private set; } = false;
    public bool IsDead { get; private set; } = false;

    public bool IsAbleToMove => !HasFallen && !IsDead;

    public bool IsPlayerCharacter;
    public bool IsNPCCharacter => !IsPlayerCharacter;

    Dictionary<MaskType, GameObject> masks = new Dictionary<MaskType, GameObject>();

    MaskType equippedMask;

    public enum AllowedFallDirections
    {
        Both,
        OnlyLeft,
        OnlyRight,
    }
    [SerializeField] AllowedFallDirections allowedFallDirections = AllowedFallDirections.Both;

    public CharacterAnimator characterAnimator;

    CharacterAnimator animator;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position); // Snap do centra tilu
        masks[MaskType.Default] = defaultMask;
        masks[MaskType.Tough] = toughMask;
        masks[MaskType.Ninja] = ninjaMask;
        masks[MaskType.Shaman] = shamanMask;
        switchMask(MaskType.Default);
    }
    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;

        transform.position = pos;
    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;


        if (!IsPathClear(targetPos))
            yield break;

        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }

    public IEnumerator MoveWithoutConstraints(Vector2 moveVec, Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;


        IsMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }


    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;


    }

    public void SetHasPoisoned()
    {
        SetHasFallen(true, true);
    }
    public void SetIsDead(bool newIsDead)
    {
        IsDead = newIsDead;
        SetHasFallen(newIsDead, false);
    }
    public void SetHasFallen(bool newFallenState)
    {
        SetHasFallen(newFallenState, false);
    }

    public void SetHasFallen(bool newFallenState, bool isPoisoned)
    {
        if (HasFallen == newFallenState)
            return;

        if (newFallenState)
        {
            AudioManager.i.PlaySfx(fallSfx);

            // fall
            if (allowedFallDirections == AllowedFallDirections.Both)
            {
                if (UnityEngine.Random.Range(0, 100) > 50)
                {
                    transform.DORotateQuaternion(Quaternion.Euler(0, 0, -90), 0.3f);
                }
                else
                {
                    transform.DORotateQuaternion(Quaternion.Euler(0, 0, 90), 0.3f);
                }
            }
            else if (allowedFallDirections == AllowedFallDirections.OnlyLeft)
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, 90), 0.3f);
            }
            else if (allowedFallDirections == AllowedFallDirections.OnlyRight)
            {
                transform.DORotateQuaternion(Quaternion.Euler(0, 0, -90), 0.3f);
            }

            if (isPoisoned)
            {
                GameController.Instance.NumNPCSPoisoned++;
                if (poisonedSprite)
                {
                    GetComponentInChildren<SpriteRenderer>().sprite = poisonedSprite;
                }
                else
                {
                    GetComponentInChildren<SpriteRenderer>().color = new Color(234 / 255.0f, 138 / 255.0f, 162 / 255.0f);
                }
                var c = GetComponentInChildren<Collider2D>();
                if (c)
                    c.enabled = false;
            }
        }
        else
        {
            transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 1); // standup
        }


        HasFallen = newFallenState;
    }

    public void SetFallAfterTime(float time)
    {
        Invoke("SetHasPoisoned", time);
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        Vector3 diff = targetPos - transform.position;
        Vector3 dir = diff.normalized;

        var collisionLayer = GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, collisionLayer) == true)
            return false;

        // Check for HoleCollider triggers (they block player but not projectiles)
        var hit = Physics2D.OverlapBox(targetPos, new Vector2(0.5f, 0.5f), 0f);
        if (hit != null && hit.GetComponent<HoleCollider>() != null)
            return false;

        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (Mathf.Abs(ydiff) > Mathf.Abs(xdiff))
            xdiff = 0;
        else
            ydiff = 0;

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
            Debug.Log("Error in Look Towards: You cant ask the character to look diagonally");
    }

    public void SetCurrentFacingDirection(FacingDirection dir)
    {
        characterAnimator.SetfacingDirection(dir);
    }
    public CharacterAnimator Animator
    {
        get => animator;
    }

    public void switchMask(MaskType newMaskType)
    {
        masks[equippedMask].SetActive(false);
        equippedMask = newMaskType;
        masks[equippedMask].SetActive(true);
    }
}
