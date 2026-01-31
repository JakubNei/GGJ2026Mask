using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovableObject : MonoBehaviour, IInteractable
{
    public bool Pullable = false;
    public bool Pushable = true;
    //public bool ForbidDiagonalMovement = true;
    Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    public bool CanPickUp()
    {
        return false;
    }
}
